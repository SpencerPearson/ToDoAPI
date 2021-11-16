using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ToDoAPI.API.Models;
using ToDoAPI.DATA.EF;
using System.Web.Http.Cors;

namespace ToDoAPI.API.Controllers
{
    [EnableCors(origins:"*", headers:"*", methods:"*")]
    public class ToDoController : ApiController
    {

        ToDoEntities db = new ToDoEntities();


        //Get (READ)
        //api/ToDo
        public IHttpActionResult GetToDos()
        {
            List<ToDoViewModel> todos = db.TodoItems.Include("Category").Select(t => new ToDoViewModel()
            {
                TodoId = t.TodoId,
                Action = t.Action,
                Done = t.Done,
                CategoryId = t.CategoryId,
                DueDate = t.DueDate,
                Category = new CategoryViewModel()
                {
                    CategoryId = t.Category.CategoryId,
                    Name = t.Category.Name,
                    Description = t.Category.Description
                    
                }
            }).ToList<ToDoViewModel>();
            if (todos.Count == 0)
            {
                return NotFound();
            }
            return Ok(todos);
        }//end GetToDos

        //ToDo/id
        //Details
        public IHttpActionResult GetToDo (int id)
        {
            ToDoViewModel todo = db.TodoItems.Include("Category").Where(t => t.TodoId == id).Select(t => new ToDoViewModel()
            {
                TodoId = t.TodoId,
                Action = t.Action,
                Done = t.Done,
                CategoryId = t.CategoryId,
                DueDate = t.DueDate,
                Category = new CategoryViewModel()
                {
                    CategoryId = t.Category.CategoryId,
                    Name = t.Category.Name,
                    Description = t.Category.Description

                }
            }).FirstOrDefault();

            if (todo == null)
                return NotFound();
            return Ok(todo);
        }

        //api/ToDo (HttpPost)
        public IHttpActionResult PostToDo(ToDoViewModel todo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Data");
            }

            TodoItem newToDo = new TodoItem()
            {
                Action = todo.Action,
                Done = todo.Done,
                CategoryId = todo.CategoryId,
                DueDate = todo.DueDate
            };

            db.TodoItems.Add(newToDo);
            db.SaveChanges();
            return Ok(newToDo);
        }

        //api/ToDo (HttpPut)
        public IHttpActionResult PutToDo(ToDoViewModel todo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Data");
            }
            TodoItem existingTodo = db.TodoItems.Where(t => t.TodoId == todo.TodoId).FirstOrDefault();

            if (existingTodo != null)
            {
                existingTodo.TodoId = todo.TodoId;
                existingTodo.Action = todo.Action;
                existingTodo.Done = todo.Done;
                existingTodo.CategoryId = todo.CategoryId;
                existingTodo.DueDate = todo.DueDate;
                db.SaveChanges();
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        //api/ToDo/id (HttpDelete)
        public IHttpActionResult DeleteToDo(int id)
        {
            TodoItem todo = db.TodoItems.Where(t => t.TodoId == id).FirstOrDefault();

            if (todo != null)
            {
                db.TodoItems.Remove(todo);
                db.SaveChanges();
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);    
        }

    }
}
