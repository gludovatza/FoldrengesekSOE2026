using FoldrengesekSOE2026.Controllers;
using FoldrengesekSOE2026.Models;
using FoldrengesekSOE2026.Services;
using FoldrengesekSOE2026.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoldrengesekSOE2026.Tests.Queries
{
    public class FeladatTests
    {
        [Fact]
        public void Feladat2_Returns_Somogy_Telepulesek_Alphabetical_Order()
        {
            // Arrange
            var ctx = TestDbFactory.CreateContext(nameof(Feladat2_Returns_Somogy_Telepulesek_Alphabetical_Order));

            ctx.Telepulesek.AddRange(
                new Telepules { ID = 1, Nev = "Kaposvár", Varmegye = "Somogy" },
                new Telepules { ID = 2, Nev = "Barcs", Varmegye = "Somogy" },
                new Telepules { ID = 3, Nev = "Siófok", Varmegye = "Somogy" },
                new Telepules { ID = 4, Nev = "Szekszárd", Varmegye = "Tolna" }
            );
            ctx.SaveChanges();

            //var controller = new FeladatokController(ctx);
            var service = new LekerdezesiFeladatok(ctx);

            //// Act
            //var result = controller.Feladat2() as ViewResult;

            //// Assert – ViewResult
            //// "0." Az eredmény nem null
            //Assert.NotNull(result);

            //var model = Assert.IsAssignableFrom<IEnumerable<string>>(result!.Model);

            //var list = model.ToList();

            var list = service.SomogyTelepulesNevek().ToList();

            // 1️. Csak Somogy
            Assert.DoesNotContain("Szekszárd", list);

            // 2️. Darabszám
            Assert.Equal(3, list.Count);

            // 3️. ABC sorrend
            Assert.Equal(
                new[] { "Barcs", "Kaposvár", "Siófok" },
                list
            );
        }

        [Fact]
        public void VarmegyeiRengesSzamok_GroupByAndCountAndOrder_Works()
        {
            // Arrange
            var ctx = TestDbFactory.CreateContext(nameof(VarmegyeiRengesSzamok_GroupByAndCountAndOrder_Works));

            ctx.Telepulesek.AddRange(
                new Telepules { ID = 1, Nev = "Kaposvár", Varmegye = "Somogy" },
                new Telepules { ID = 2, Nev = "Siófok", Varmegye = "Somogy" },
                new Telepules { ID = 3, Nev = "Szekszárd", Varmegye = "Tolna" }
            );

            // Somogyhoz 3 naplóbejegyzés, Tolnához 1 naplóbejegyzés
            ctx.Naplok.AddRange(
                new Naplo { ID = 1, TelepulesID = 1, Datum = new DateTime(2022, 1, 1), Ido = new TimeSpan(10, 0, 0), Magnitudo = 1.1, Intenzitas = 2.2 },
                new Naplo { ID = 2, TelepulesID = 1, Datum = new DateTime(2022, 1, 2), Ido = new TimeSpan(11, 0, 0), Magnitudo = 1.2, Intenzitas = 2.3 },
                new Naplo { ID = 3, TelepulesID = 2, Datum = new DateTime(2022, 1, 3), Ido = new TimeSpan(12, 0, 0), Magnitudo = 1.3, Intenzitas = 2.4 },
                new Naplo { ID = 4, TelepulesID = 3, Datum = new DateTime(2022, 1, 4), Ido = new TimeSpan(13, 0, 0), Magnitudo = 1.4, Intenzitas = 2.5 }
            );

            ctx.SaveChanges();

            var service = new LekerdezesiFeladatok(ctx);

            // Act
            var list = service.VarmegyeiRengesSzamok().ToList();

            // Assert
            Assert.Equal(2, list.Count);

            // OrderByDescending miatt Somogy legyen az első (3 db)
            Assert.Equal("Somogy", list[0].Varmegye);
            Assert.Equal(3, list[0].Count);

            Assert.Equal("Tolna", list[1].Varmegye);
            Assert.Equal(1, list[1].Count);
        }

        [Fact]
        public void LegnagyobbMagnitudo_ReturnsMaxMagnitudeTelepulesName()
        {
            // Arrange
            var ctx = TestDbFactory.CreateContext(nameof(LegnagyobbMagnitudo_ReturnsMaxMagnitudeTelepulesName));

            ctx.Telepulesek.AddRange(
                new Telepules { ID = 1, Nev = "Kaposvár", Varmegye = "Somogy" },
                new Telepules { ID = 2, Nev = "Szekszárd", Varmegye = "Tolna" }
            );

            ctx.Naplok.AddRange(
                new Naplo { ID = 1, TelepulesID = 1, Datum = new DateTime(2022, 1, 1), Ido = new TimeSpan(10, 0, 0), Magnitudo = 1.10, Intenzitas = 2.0 },
                new Naplo { ID = 2, TelepulesID = 2, Datum = new DateTime(2022, 1, 2), Ido = new TimeSpan(11, 0, 0), Magnitudo = 3.50, Intenzitas = 4.0 },
                new Naplo { ID = 3, TelepulesID = 1, Datum = new DateTime(2022, 1, 3), Ido = new TimeSpan(12, 0, 0), Magnitudo = 2.20, Intenzitas = 3.0 }
            );

            ctx.SaveChanges();

            var service = new LekerdezesiFeladatok(ctx);

            // Act
            var result = service.LegnagyobbMagnitudo();

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Szekszárd", result!.Nev);
            Assert.Equal(3.50, result.Magnitudo);
        }

        [Fact]
        public void LegnagyobbMagnitudo_WhenNoData_ReturnsNull()
        {
            var ctx = TestDbFactory.CreateContext(nameof(LegnagyobbMagnitudo_WhenNoData_ReturnsNull));
            var service = new LekerdezesiFeladatok(ctx);

            var result = service.LegnagyobbMagnitudo();

            Assert.Null(result);
        }

        [Fact]
        public void AligErzekelheto2022_FiltersByYearAndIntensityInclusive_AndSortsByDateAsc()
        {
            // Arrange
            var ctx = TestDbFactory.CreateContext(nameof(AligErzekelheto2022_FiltersByYearAndIntensityInclusive_AndSortsByDateAsc));

            ctx.Telepulesek.AddRange(
                new Telepules { ID = 1, Nev = "Kaposvár", Varmegye = "Somogy" },
                new Telepules { ID = 2, Nev = "Szekszárd", Varmegye = "Tolna" }
            );

            ctx.Naplok.AddRange(
                // 2022, benne van (alsó határ)
                new Naplo { ID = 1, TelepulesID = 1, Datum = new DateTime(2022, 1, 10), Ido = new TimeSpan(10, 0, 0), Magnitudo = 1.0, Intenzitas = 2.0 },

                // 2022, benne van (felső határ)
                new Naplo { ID = 2, TelepulesID = 2, Datum = new DateTime(2022, 1, 5), Ido = new TimeSpan(11, 0, 0), Magnitudo = 1.0, Intenzitas = 3.0 },

                // 2022, KIESIK (túl kicsi)
                new Naplo { ID = 3, TelepulesID = 1, Datum = new DateTime(2022, 2, 1), Ido = new TimeSpan(12, 0, 0), Magnitudo = 1.0, Intenzitas = 1.9 },

                // 2022, KIESIK (túl nagy)
                new Naplo { ID = 4, TelepulesID = 2, Datum = new DateTime(2022, 3, 1), Ido = new TimeSpan(9, 0, 0), Magnitudo = 1.0, Intenzitas = 3.1 },

                // Más év, KIESIK
                new Naplo { ID = 5, TelepulesID = 2, Datum = new DateTime(2021, 12, 31), Ido = new TimeSpan(9, 0, 0), Magnitudo = 1.0, Intenzitas = 2.5 }
            );

            ctx.SaveChanges();

            var service = new LekerdezesiFeladatok(ctx);

            // Act
            var list = service.AligErzekelheto2022().ToList();

            // Assert: csak 2 rekord marad (ID 1 és 2)
            Assert.Equal(2, list.Count);

            // dátum szerint növekvő: 2022-01-05, majd 2022-01-10
            Assert.Equal(new DateTime(2022, 1, 5), list[0].Datum);
            Assert.Equal("Szekszárd", list[0].Nev);
            Assert.Equal(3.0, list[0].Intenzitas);

            Assert.Equal(new DateTime(2022, 1, 10), list[1].Datum);
            Assert.Equal("Kaposvár", list[1].Nev);
            Assert.Equal(2.0, list[1].Intenzitas);
        }

        [Fact]
        public void Top3Evk_3nalNagyobbIntenzitas_ReturnsTop3Years_ByCountDesc()
        {
            // Arrange
            var ctx = TestDbFactory.CreateContext(nameof(Top3Evk_3nalNagyobbIntenzitas_ReturnsTop3Years_ByCountDesc));

            // Kell legalább 1 település, hogy a FK rendben legyen
            ctx.Telepulesek.Add(new Telepules { ID = 1, Nev = "Kaposvár", Varmegye = "Somogy" });

            ctx.Naplok.AddRange(
                // 2020: 3 db > 3.0
                new Naplo { ID = 1, TelepulesID = 1, Datum = new DateTime(2020, 1, 1), Ido = new TimeSpan(10, 0, 0), Magnitudo = 1.0, Intenzitas = 3.1 },
                new Naplo { ID = 2, TelepulesID = 1, Datum = new DateTime(2020, 2, 1), Ido = new TimeSpan(10, 0, 0), Magnitudo = 1.0, Intenzitas = 3.5 },
                new Naplo { ID = 3, TelepulesID = 1, Datum = new DateTime(2020, 3, 1), Ido = new TimeSpan(10, 0, 0), Magnitudo = 1.0, Intenzitas = 4.0 },

                // 2021: 1 db > 3.0 + 1 db pont 3.0 (nem számít, mert > 3.0 kell!)
                new Naplo { ID = 4, TelepulesID = 1, Datum = new DateTime(2021, 1, 1), Ido = new TimeSpan(10, 0, 0), Magnitudo = 1.0, Intenzitas = 3.2 },
                new Naplo { ID = 5, TelepulesID = 1, Datum = new DateTime(2021, 2, 1), Ido = new TimeSpan(10, 0, 0), Magnitudo = 1.0, Intenzitas = 3.0 }, // kiesik

                // 2022: 2 db > 3.0
                new Naplo { ID = 6, TelepulesID = 1, Datum = new DateTime(2022, 1, 1), Ido = new TimeSpan(10, 0, 0), Magnitudo = 1.0, Intenzitas = 3.1 },
                new Naplo { ID = 7, TelepulesID = 1, Datum = new DateTime(2022, 2, 1), Ido = new TimeSpan(10, 0, 0), Magnitudo = 1.0, Intenzitas = 3.9 },

                // 2019: 4 db > 3.0  (ez lesz az 1. hely)
                new Naplo { ID = 8, TelepulesID = 1, Datum = new DateTime(2019, 1, 1), Ido = new TimeSpan(10, 0, 0), Magnitudo = 1.0, Intenzitas = 3.1 },
                new Naplo { ID = 9, TelepulesID = 1, Datum = new DateTime(2019, 2, 1), Ido = new TimeSpan(10, 0, 0), Magnitudo = 1.0, Intenzitas = 3.2 },
                new Naplo { ID = 10, TelepulesID = 1, Datum = new DateTime(2019, 3, 1), Ido = new TimeSpan(10, 0, 0), Magnitudo = 1.0, Intenzitas = 3.3 },
                new Naplo { ID = 11, TelepulesID = 1, Datum = new DateTime(2019, 4, 1), Ido = new TimeSpan(10, 0, 0), Magnitudo = 1.0, Intenzitas = 3.4 }
            );

            ctx.SaveChanges();

            var service = new LekerdezesiFeladatok(ctx);

            // Act
            var list = service.Top3Ev_3nalNagyobbIntenzitassal().ToList();

            // Assert
            Assert.Equal(3, list.Count);

            // sorrend: 2019 (4), 2020 (3), 2022 (2)
            Assert.Equal(new Feladat6ViewModel { Year = 2019, Count = 4 }.Year, list[0].Year);
            Assert.Equal(4, list[0].Count);

            Assert.Equal(2020, list[1].Year);
            Assert.Equal(3, list[1].Count);

            Assert.Equal(2022, list[2].Year);
            Assert.Equal(2, list[2].Count);

            // 2021 csak 1 db > 3.0, ezért nem fér be a top3-ba
            Assert.DoesNotContain(list, x => x.Year == 2021);
        }
    }
}
