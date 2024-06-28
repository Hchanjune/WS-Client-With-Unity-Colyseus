using System;
using Colyseus.Schema;

public partial class Player : Schema {
		[Colyseus.Schema.Type(0, "string")]
		public string id = default(string);

		[Colyseus.Schema.Type(1, "string")]
		public string name = default(string);

		[Colyseus.Schema.Type(2, "int32")]
		public int score = default(int);

		/*
		 * Support for individual property change callbacks below...
		 */

		protected event PropertyChangeHandler<string> __idChange;
		public Action OnIdChange(PropertyChangeHandler<string> __handler, bool __immediate = true) {
			if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
			__callbacks.AddPropertyCallback(nameof(this.id));
			__idChange += __handler;
			if (__immediate && this.id != default(string)) { __handler(this.id, default(string)); }
			return () => {
				__callbacks.RemovePropertyCallback(nameof(id));
				__idChange -= __handler;
			};
		}

		protected event PropertyChangeHandler<string> __nameChange;
		public Action OnNameChange(PropertyChangeHandler<string> __handler, bool __immediate = true) {
			if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
			__callbacks.AddPropertyCallback(nameof(this.name));
			__nameChange += __handler;
			if (__immediate && this.name != default(string)) { __handler(this.name, default(string)); }
			return () => {
				__callbacks.RemovePropertyCallback(nameof(name));
				__nameChange -= __handler;
			};
		}

		protected event PropertyChangeHandler<int> __scoreChange;
		public Action OnScoreChange(PropertyChangeHandler<int> __handler, bool __immediate = true) {
			if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
			__callbacks.AddPropertyCallback(nameof(this.score));
			__scoreChange += __handler;
			if (__immediate && this.score != default(int)) { __handler(this.score, default(int)); }
			return () => {
				__callbacks.RemovePropertyCallback(nameof(score));
				__scoreChange -= __handler;
			};
		}

		protected override void TriggerFieldChange(DataChange change) {
			switch (change.Field) {
				case nameof(id): __idChange?.Invoke((string) change.Value, (string) change.PreviousValue); break;
				case nameof(name): __nameChange?.Invoke((string) change.Value, (string) change.PreviousValue); break;
				case nameof(score): __scoreChange?.Invoke((int) change.Value, (int) change.PreviousValue); break;
				default: break;
			}
		}
	}