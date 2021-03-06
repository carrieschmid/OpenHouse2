using System.Collections.Generic;
using Domain;

namespace Application.Profiles {
    public class Profile {
        public string DisplayName { get; set; }
        public string Username { get; set; }
        public string Interests { get; set; }
        public string BgCheck { get; set; }
        public string FirstAid { get; set; }
        public string Terms { get; set; }
        public string Image { get; set; }
        public string Bio { get; set; }

        public virtual ICollection<Photo> Photos { get; set; }

    }
}