public class CreateCartDto
{
    public int IdUzytkownika { get; set; }
    public DateTime DataOstatniejModyfikacji { get; set; }
    public List<CartItemDto> Pozycje { get; set; } = new();
}