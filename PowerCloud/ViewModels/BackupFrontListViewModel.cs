using PowerCloud.Models;

namespace PowerCloud.ViewModels
{
    public class BackupFrontListViewModel
    {
        bool includeEmptyGroups;
        public List<ImgGroup> Animals { get; private set; } = new List<ImgGroup>();

        public BackupFrontListViewModel(bool emptyGroups = false)
        {
            includeEmptyGroups = emptyGroups;
            CreateAnimalsCollection();
        }

        void CreateAnimalsCollection()
        {
            if (includeEmptyGroups)
                Animals.Add(new ImgGroup("Aardvarks", new List<BackupItem>()));

            Animals.Add(new ImgGroup("2020/08", new List<BackupItem>
            {
                new BackupItem
                {
                    ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/6/6c/Sloth_Bear_Washington_DC.JPG/320px-Sloth_Bear_Washington_DC.JPG"
                },
                new BackupItem
                {
                    ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/a/a6/Sitting_sun_bear.jpg/319px-Sitting_sun_bear.jpg"
                },
            }));

            Animals.Add(new ImgGroup("2020/07", new List<BackupItem>
            {
                new BackupItem
                {
                    ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/9/9b/Gustav_chocolate.jpg/168px-Gustav_chocolate.jpg"
                },
                new BackupItem
                {
                    ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/d/d3/Bex_Arabian_Mau.jpg"
                },
                new BackupItem
                {
                    ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/b/ba/Paintedcats_Red_Star_standing.jpg/187px-Paintedcats_Red_Star_standing.jpg"
                },
                new BackupItem
                {
                    ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/5/5d/Adult_Scottish_Fold.jpg/240px-Adult_Scottish_Fold.jpg"
                },
                new BackupItem
                {
                    ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/e/e8/Sphinx2_July_2006.jpg/180px-Sphinx2_July_2006.jpg"
                }
            }));

            Animals.Add(new ImgGroup("2020/06", new List<BackupItem>
            {

                new BackupItem
                {
                    ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/6/69/Afghane.jpg"
                },
                new BackupItem
                {
                    ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/d/d7/Boston-terrier-carlos-de.JPG/320px-Boston-terrier-carlos-de.JPG"
                },
                new BackupItem
                {
                    ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/7/79/Spoonsced.jpg"
                },
                new BackupItem
                {
                    ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/9/98/Eurohound.jpg"
                },
                new BackupItem
                {
                    ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/5/56/IrishTerrierSydenhamHillWoods.jpg/180px-IrishTerrierSydenhamHillWoods.jpg"
                },
                new BackupItem
                {
                    ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/6/69/Afghane.jpg"
                },
                new BackupItem
                {
                    ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/d/d7/Boston-terrier-carlos-de.JPG/320px-Boston-terrier-carlos-de.JPG"
                },
                new BackupItem
                {
                    ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/7/79/Spoonsced.jpg"
                },
                new BackupItem
                {
                    ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/9/98/Eurohound.jpg"
                },
                new BackupItem
                {
                    ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/5/56/IrishTerrierSydenhamHillWoods.jpg/180px-IrishTerrierSydenhamHillWoods.jpg"
                },
                new BackupItem
                {
                    ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/6/69/Afghane.jpg"
                },
                new BackupItem
                {
                    ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/d/d7/Boston-terrier-carlos-de.JPG/320px-Boston-terrier-carlos-de.JPG"
                },
                new BackupItem
                {
                    ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/7/79/Spoonsced.jpg"
                },
                new BackupItem
                {
                    ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/9/98/Eurohound.jpg"
                },
                new BackupItem
                {
                    ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/5/56/IrishTerrierSydenhamHillWoods.jpg/180px-IrishTerrierSydenhamHillWoods.jpg"
                },
                new BackupItem
                {
                    ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/6/69/Afghane.jpg"
                },
                new BackupItem
                {
                    ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/d/d7/Boston-terrier-carlos-de.JPG/320px-Boston-terrier-carlos-de.JPG"
                },
                new BackupItem
                {
                    ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/7/79/Spoonsced.jpg"
                },
                new BackupItem
                {
                    ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/9/98/Eurohound.jpg"
                },
                new BackupItem
                {
                    ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/5/56/IrishTerrierSydenhamHillWoods.jpg/180px-IrishTerrierSydenhamHillWoods.jpg"
                },
            }));

        }
    }
}
