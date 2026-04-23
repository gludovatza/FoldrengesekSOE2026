using FoldrengesekSOE2026.ViewModels;

namespace FoldrengesekSOE2026.Services
{
    public interface ILekerdezesiFeladatok
    {
        IQueryable<string> SomogyTelepulesNevek();
        IQueryable<Feladat3ViewModel> VarmegyeiRengesSzamok();
        Feladat4ViewModel? LegnagyobbMagnitudo();
        IQueryable<Feladat5ViewModel> AligErzekelheto2022();
        IQueryable<Feladat6ViewModel> Top3Ev_3nalNagyobbIntenzitassal();
    }
}
