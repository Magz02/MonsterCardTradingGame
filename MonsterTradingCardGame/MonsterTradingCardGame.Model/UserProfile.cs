using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame.Model {
    public class UserProfile {
        public UserProfile(string name, string bio, string image) {
            this.Name = name;
            this.Bio = bio;
            this.Image = image;
        }

        public string Name { get; set; }
        public string Bio { get; set; }
        public string Image { get; set; }
    }
}
