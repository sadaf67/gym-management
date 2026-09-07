using Tahila.Domain.Entities;

namespace Tahila.Web.Models;

public class HomeViewModel
{
    public List<Slider> Sliders { get; set; } = new();
    public List<Coach> Coaches { get; set; } = new();
    public List<GymClass> Classes { get; set; } = new();
    public List<Plan> Plans { get; set; } = new();
    public SiteSetting Settings { get; set; } = new();
}
