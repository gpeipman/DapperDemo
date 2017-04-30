using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using DapperDemo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DapperDemo.Controllers
{
    public class HomeController : Controller
    {
        private readonly string _connectionString;

        public HomeController(IConfiguration configuration)
        {
            _connectionString = configuration.GetValue<string>("ConnectionStrings:Default");
        }

        private IDbConnection GetConnection()
        {
            var connection = new SqlConnection(_connectionString);
            connection.Open();

            return connection;
        }
        public IActionResult Index()
        {
            using (var connection = GetConnection())
            {
                var items = connection.Query<ToDoItem>("select * from ToDoItem");
                return View(items);
            }
        }

        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            using (var connection = GetConnection())
            {
                var item = connection.QueryFirst<ToDoItem>("select * from ToDoItem where Id=@Id", new { Id = id });
                return View(item);
            }
        }

        public IActionResult Create()
        {
            return View("Edit", new ToDoItem());
        }

        public IActionResult Edit(int id)
        {
            using (var connection = GetConnection())
            {
                var item = connection.QueryFirst<ToDoItem>("select * from ToDoItem where Id=@id", new { Id = id });

                return View(item);
            }
        }

        [HttpPost]
        public IActionResult Edit(ToDoItem model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (var connection = GetConnection())
            {
                if(model.Id == 0)
                    connection.Execute("insert into ToDoItem (Title, Done) values(@Title, @Done)", model);
                else 
                    connection.Execute("update ToDoItem set Title=@Title, Done=@Done where Id=@Id", model);

                return RedirectToAction("Index");
            }
        }

        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            using (var connection = GetConnection())
            {
                var item = connection.QueryFirst<ToDoItem>("select * from ToDoItem where Id=@Id", new { Id = id });
                if(item == null)
                {
                    return NotFound();
                }

                return View(item);
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            using (var connection = GetConnection())
            {
                connection.Execute("delete from ToDoItem where Id=@Id", new { Id = id });

                return RedirectToAction("Index");
            }
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
