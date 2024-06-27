﻿using Colyseus.Schema;

namespace DefaultNamespace
{
    public partial class LobbyState: Schema
    {
        [Type(0, "map", typeof(MapSchema<Player>))]
        public MapSchema<Player> players = new MapSchema<Player>();
    }
}