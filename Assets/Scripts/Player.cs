using Colyseus.Schema;

namespace DefaultNamespace
{
    public partial class Player : Schema
    {
        [Type(0, "string")]
        public string id = "";

        [Type(1, "string")]
        public string name = "";

        [Type(2, "int32")]
        public int score = 0;
    }
}