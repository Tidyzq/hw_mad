using SQLitePCL;
using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace hw6 {
    public class TodoItemCollection : ObservableCollection<TodoItem> {

        static private TodoItemCollection collection;
        static public TodoItemCollection Collection {
            get {
                if (collection == null) {
                    collection = new TodoItemCollection();
                }
                return collection;
            }
        }

        private TodoItemCollection() {
            var db = App.conn;
            Search("");
        }

        new public void Add(TodoItem item) {
            var db = App.conn;
            using (var statement = db.Prepare("INSERT INTO TodoItem (Name, Detail, DueDate, Finished) VALUES (?, ?, ?, ?);")) {
                statement.Bind(1, item.Name);
                statement.Bind(2, item.Detail);
                Debug.WriteLine(item.DueDate.Date.ToString());
                statement.Bind(3, string.Format("{0}/{1}/{2}", item.DueDate.Year, item.DueDate.Month, item.DueDate.Day));
                statement.Bind(4, item.Finished == true ? 1 : 0);
                statement.Step();
            }
            base.Add(new TodoItem().copy(item));
        }

        new public void Remove(TodoItem item) {
            var db = App.conn;
            using (var statement = db.Prepare("DELETE FROM TodoItem WHERE Id = ?;")) {
                statement.Bind(1, item.Id);
                statement.Step();
            }
            base.Remove(item);
        }

        public void Update(TodoItem item) {
            var db = App.conn;
            using (var statement = db.Prepare("UPDATE TodoItem SET Name = ?, Detail = ?, DueDate = ?, Finished = ? WHERE Id = ?;")) {
                statement.Bind(1, item.Name);
                statement.Bind(2, item.Detail);
                statement.Bind(3, string.Format("{0}/{1}/{2}", item.DueDate.Year, item.DueDate.Month, item.DueDate.Day));
                statement.Bind(4, item.Finished == true ? 1 : 0);
                statement.Bind(5, item.Id);
                statement.Step();
            }
        }

        public void Search(string query) {
            var db = App.conn;
            this.Clear();
            if (query == "") {
                using (var statement = db.Prepare("SELECT Id, Name, Detail, DueDate, Finished FROM TodoItem;")) {
                    while (statement.Step() == SQLiteResult.ROW) {
                        var id = (Int64)statement[0];
                        var name = (string)statement[1];
                        var detail = (string)statement[2];
                        Regex regex = new Regex(@"(\d+)/(\d+)/(\d+)");
                        Match match = regex.Match((string)statement[3]);
                        var year = Int32.Parse(match.Groups[1].ToString());
                        var month = Int32.Parse(match.Groups[2].ToString());
                        var day = Int32.Parse(match.Groups[3].ToString());
                        var dueDate = new DateTimeOffset(new DateTime(year, month, day));
                        var finished = (Int64)statement[4] == 1 ? true : false;
                        base.Add(new TodoItem(id, name, detail, dueDate, finished));
                    }
                }
            } else {
                Regex regex = new Regex(@"\w+");
                var matches = regex.Matches(query);
                string sql = "";
                foreach (var match in matches) {
                    sql += (sql.Length > 0 ? " AND " : "") + string.Format("Name GLOB '*{0}*' OR Detail GLOB '*{0}*' OR DueDate GLOB '*{0}*'", match.ToString());
                }
                using (var statement = db.Prepare("SELECT Id, Name, Detail, DueDate, Finished FROM TodoItem WHERE " + sql)) {
                    while (statement.Step() == SQLiteResult.ROW) {
                        var id = (Int64)statement[0];
                        var name = (string)statement[1];
                        Debug.WriteLine(name);
                        var detail = (string)statement[2];
                        regex = new Regex(@"(\d+)/(\d+)/(\d+)");
                        Match match = regex.Match((string)statement[3]);
                        var year = Int32.Parse(match.Groups[1].ToString());
                        var month = Int32.Parse(match.Groups[2].ToString());
                        var day = Int32.Parse(match.Groups[3].ToString());
                        var dueDate = new DateTimeOffset(new DateTime(year, month, day));
                        var finished = (Int64)statement[4] == 1 ? true : false;
                        base.Add(new TodoItem(id, name, detail, dueDate, finished));
                    }
                }
            }
        }
    }
}
