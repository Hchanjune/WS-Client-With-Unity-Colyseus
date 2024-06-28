using System;
using Colyseus.Schema;

public partial class LobbyChatRoomListState : Schema {
		[Colyseus.Schema.Type(0, "string")]
		public string roomId = default(string);

		[Colyseus.Schema.Type(1, "string")]
		public string roomOwner = default(string);

		[Colyseus.Schema.Type(2, "string")]
		public string roomName = default(string);

		[Colyseus.Schema.Type(3, "string")]
		public string created = default(string);

		/*
		 * Support for individual property change callbacks below...
		 */

		protected event PropertyChangeHandler<string> __roomIdChange;
		public Action OnRoomIdChange(PropertyChangeHandler<string> __handler, bool __immediate = true) {
			if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
			__callbacks.AddPropertyCallback(nameof(this.roomId));
			__roomIdChange += __handler;
			if (__immediate && this.roomId != default(string)) { __handler(this.roomId, default(string)); }
			return () => {
				__callbacks.RemovePropertyCallback(nameof(roomId));
				__roomIdChange -= __handler;
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

		protected event PropertyChangeHandler<string> __createdChange;
		public Action OnCreatedChange(PropertyChangeHandler<string> __handler, bool __immediate = true) {
			if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
			__callbacks.AddPropertyCallback(nameof(this.created));
			__createdChange += __handler;
			if (__immediate && this.created != default(string)) { __handler(this.created, default(string)); }
			return () => {
				__callbacks.RemovePropertyCallback(nameof(created));
				__createdChange -= __handler;
			};
		}

		protected override void TriggerFieldChange(DataChange change) {
			switch (change.Field) {
				case nameof(roomId): __roomIdChange?.Invoke((string) change.Value, (string) change.PreviousValue); break;
				case nameof(roomOwner): __roomOwnerChange?.Invoke((string) change.Value, (string) change.PreviousValue); break;
				case nameof(roomName): __roomNameChange?.Invoke((string) change.Value, (string) change.PreviousValue); break;
				case nameof(created): __createdChange?.Invoke((string) change.Value, (string) change.PreviousValue); break;
				default: break;
			}
		}
	}