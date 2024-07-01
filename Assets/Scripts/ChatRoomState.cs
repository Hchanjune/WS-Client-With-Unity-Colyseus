using System;
using Colyseus.Schema;

public partial class ChatRoomState : Schema {
		[Colyseus.Schema.Type(0, "string")]
		public string roomName = default(string);

		[Colyseus.Schema.Type(1, "string")]
		public string roomOwner = default(string);

		[Colyseus.Schema.Type(2, "map", typeof(MapSchema<ChatRoomPlayer>))]
		public MapSchema<ChatRoomPlayer> chatRoomPlayers = new MapSchema<ChatRoomPlayer>();

		/*
		 * Support for individual property change callbacks below...
		 */

		protected event PropertyChangeHandler<string> __roomNameChange;
		public Action OnRoomNameChange(PropertyChangeHandler<string> __handler, bool __immediate = true) {
			if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
			__callbacks.AddPropertyCallback(nameof(this.roomName));
			__roomNameChange += __handler;
			if (__immediate && this.roomName != default(string)) { __handler(this.roomName, default(string)); }
			return () => {
				__callbacks.RemovePropertyCallback(nameof(roomName));
				__roomNameChange -= __handler;
			};
		}

		protected event PropertyChangeHandler<string> __roomOwnerChange;
		public Action OnRoomOwnerChange(PropertyChangeHandler<string> __handler, bool __immediate = true) {
			if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
			__callbacks.AddPropertyCallback(nameof(this.roomOwner));
			__roomOwnerChange += __handler;
			if (__immediate && this.roomOwner != default(string)) { __handler(this.roomOwner, default(string)); }
			return () => {
				__callbacks.RemovePropertyCallback(nameof(roomOwner));
				__roomOwnerChange -= __handler;
			};
		}

		protected event PropertyChangeHandler<MapSchema<ChatRoomPlayer>> __playersChange;
		public Action OnPlayersChange(PropertyChangeHandler<MapSchema<ChatRoomPlayer>> __handler, bool __immediate = true) {
			if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
			__callbacks.AddPropertyCallback(nameof(this.chatRoomPlayers));
			__playersChange += __handler;
			if (__immediate && this.chatRoomPlayers != null) { __handler(this.chatRoomPlayers, null); }
			return () => {
				__callbacks.RemovePropertyCallback(nameof(chatRoomPlayers));
				__playersChange -= __handler;
			};
		}

		protected override void TriggerFieldChange(DataChange change) {
			switch (change.Field) {
				case nameof(roomName): __roomNameChange?.Invoke((string) change.Value, (string) change.PreviousValue); break;
				case nameof(roomOwner): __roomOwnerChange?.Invoke((string) change.Value, (string) change.PreviousValue); break;
				case nameof(chatRoomPlayers): __playersChange?.Invoke((MapSchema<ChatRoomPlayer>) change.Value, (MapSchema<ChatRoomPlayer>) change.PreviousValue); break;
				default: break;
			}
		}
	}