using prn231Flower.Data.Models;

namespace prn231Flower.API.ViewModel;

public class UserVM
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
    public List<Flower> Flowers { get; set; }
}
