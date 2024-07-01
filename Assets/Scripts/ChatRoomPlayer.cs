
using Colyseus.Schema;

public partial class ChatRoomPlayer: Schema
{
    [Colyseus.Schema.Type(0, "string")]
    public string id = default(string);

    [Colyseus.Schema.Type(1, "string")]
    public string name = default(string);

    [Colyseus.Schema.Type(2, "boolean")]
    public bool isOwner = default(bool);
    
    [Colyseus.Schema.Type(3, "boolean")]
    public bool isReady = default(bool);
}
