import { ref, computed } from "vue"
import api from "@/services/axios.js"

const STORAGE_KEY = "shopping_cart"
const TOKEN_KEY = "token"

const getUserId = () => {
  try {
    const user = JSON.parse(localStorage.getItem("user") || "null")
    const id = Number(user?.id ?? user?.Id)
    return Number.isInteger(id) && id > 0 ? id : null
  } catch {
    return null
  }
}

let cartLoadPromise = null
const cartId = ref(null)
const cartUserId = ref(null)

export const isLoggedIn = ref(localStorage.getItem(TOKEN_KEY) !== null)
export const cartItems = ref([])
export const isLocalDelivery = ref(false)
export const selectedDelivery = ref({
  id: 0,
  name: "",
  price: 0.0,
  icon: "fa-solid fa-truck",
})
export const addressForm = ref({
  firstName: "",
  lastName: "",
  email: "",
  phone: "",
  street: "",
  buildingNo: "",
  aptNo: "",
  postalCode: "",
  city: "",
})
export const refreshLoggedInStatus = () => {
  isLoggedIn.value = localStorage.getItem(TOKEN_KEY) !== null
}
export const shippingCost = computed(() => {
  const address = addressForm.value
  const isCartEmpty = !cartItems.value || cartItems.value.length === 0
  const isLocal = selectedDelivery.value.id === 3
  const isPostalValid = /^\d{2}-\d{3}$/.test(address.postalCode)

  if (
    isCartEmpty ||
    isLocal ||
    address.city === "" ||
    address.street === "" ||
    address.buildingNo === "" ||
    !isPostalValid
  ) {
    return 0
  }

  return selectedDelivery.value ? selectedDelivery.value.price : 0
})
const syncLocalStorage = () => {
  if (!isLoggedIn.value) {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(cartItems.value))
  }
}

export const cartSum = computed(() => {
  return cartItems.value.reduce((acc, item) => {
    const price = item.product?.cena || 0
    const qty = item.ilosc || 0
    return acc + price * qty
  }, 0)
})

export const cartCount = computed(() => {
  return cartItems.value.reduce((acc, item) => {
    const qty = item.ilosc || 0
    return acc + qty
  }, 0)
})

const createRemoteCart = async (currentUserId) => {
  const payload = {
    idUzytkownika: currentUserId,
    pozycje: [],
  }

  const { data } = await api.post("cart", payload)
  cartId.value = data.idKoszyka
  cartUserId.value = currentUserId
  cartItems.value = data.pozycje || []
  return cartItems.value
}

export const getCart = async () => {
  refreshLoggedInStatus()
  if (!isLoggedIn.value) {
    const localData = localStorage.getItem(STORAGE_KEY)
    cartItems.value = localData ? JSON.parse(localData) : []
    cartId.value = null
    cartUserId.value = null
    return cartItems.value
  }

  if (cartLoadPromise) return cartLoadPromise

  const currentUserId = getUserId()
  if (!currentUserId) {
    throw new Error("The logged-in user's ID is missing from local storage.")
  }

  cartLoadPromise = (async () => {
    try {
      const { data } = await api.get(`cart/latest/${currentUserId}`)
      cartItems.value = data.pozycje || []
      cartId.value = data.idKoszyka
      cartUserId.value = currentUserId
      return cartItems.value
    } catch (error) {
      if (error.response?.status === 404) {
        return await createRemoteCart(currentUserId)
      }
      throw error
    }
  })()

  try {
    return await cartLoadPromise
  } finally {
    cartLoadPromise = null
  }
}

export const updateQuantity = async (product, delta, showAlert) => {
  refreshLoggedInStatus()
  if (!product) return false

  try {
    const item = cartItems.value.find(
      (i) => i.idProduktu === product.idProduktu,
    )
    const currentQty = item ? item.ilosc : 0
    const newQty = currentQty + delta
    const isValid = await isItemQuantityValid(product.idProduktu, newQty)
    if (!isValid) {
      showAlert({
        type: "warning",
        message: `Max stock`,
        duration: 800,
      })
      return false
    }
    if (!isLoggedIn.value) {
      if (item) {
        item.ilosc = newQty
      } else {
        cartItems.value.push({
          idProduktu: product.idProduktu,
          ilosc: 1,
          product,
        })
      }
      syncLocalStorage()
    } else {
      const payload = {
        idPozycji: item.idPozycji,
        IdKoszyka: cartId.value,
        IdProduktu: product.idProduktu,
        Ilosc: delta,
      }
      await api.put(`cartitems`, payload)
      await getCart()
    }

    return true
  } catch (error) {
    console.error("Update error:", error)
    return false
  }
}
export const addToCart = async (product, showAlert) => {
  refreshLoggedInStatus()
  try {
    if (!product) throw new Error("No product was provided.")

    const productId = Number(product.idProduktu ?? product.productId ?? product.id)
    if (!Number.isInteger(productId) || productId <= 0) {
      throw new Error("The product ID is missing or invalid.")
    }

    const cartProduct = product.idProduktu === productId
      ? product
      : { ...product, idProduktu: productId }

    const currentUserId = getUserId()
    if (
      isLoggedIn.value &&
      (!cartId.value || cartUserId.value !== currentUserId)
    ) {
      await getCart()
    }
    const exists = cartItems.value.some(
      (i) => i.idProduktu === productId,
    )

    if (exists) {
      return await updateQuantity(cartProduct, 1, showAlert)
    }
    const isValid = await isItemQuantityValid(productId, 1)
    if (!isValid) {
      showAlert({
        type: "error",
        message: "Product is not avaliable",
        duration: 1000,
      })

      return
    }
    if (!isLoggedIn.value) {
      cartItems.value.push({
        idProduktu: productId,
        ilosc: 1,
        product: cartProduct,
      })
      syncLocalStorage()
    } else {
      await api.post("cartitems", {
        idKoszyka: cartId.value,
        idProduktu: productId,
        ilosc: 1,
      })
      await getCart()
    }

    showAlert({ type: "success", message: "Added to cart", duration: 1000 })
  } catch (error) {
    console.error("Add to cart failed:", error.response?.data || error)
    showAlert({
      type: "error",
      message: "Failed to add product",
      duration: 1000,
    })
  }
}

export const removeFromCart = async (itemId, showAlert) => {
  if (!isLoggedIn.value) {
    cartItems.value = cartItems.value.filter((i) => i.idProduktu !== itemId)
    syncLocalStorage()
  } else {
    try {
      await api.delete(`cartitems/${itemId}`)
      await getCart()
    } catch (e) {
      console.error(e)
    }
  }

  showAlert({ type: "success", message: "Item removed", duration: 1000 })
}
const isItemQuantityValid = async (productId, newQty) => {
  if (newQty <= 0) return false
  const { data: product } = await api.get(`products/${productId}`)
  return newQty <= product.stanMagazynowy
}
export const clearCart = () => {
  cartItems.value = []
  if (isLoggedIn.value) api.delete(`cart/${cartId.value}`)
  else localStorage.removeItem(STORAGE_KEY)
}
