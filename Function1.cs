using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace FunctionApp2
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            List<Employee> employees = new List<Employee>();
            // if get request
            if (req.Method.ToLower() == "get")
            {
                // if there exist query string
                if (req.Query.Keys.Count > 0)
                {
                    // read the value for query string
                    int id = Convert.ToInt32(req.Query["id"]);
                    if (id > 0)
                    {
                        employees = (from e in new Employees()
                                     where e.Id == id
                                     select e).ToList();
                    }
                    else
                    { 
                        employees = new Employees();
                    }
                }
                else
                {
                    employees = new Employees();
                }
            }
            // if post request
            if (req.Method.ToLower() == "post")
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                Employee emp = JsonConvert.DeserializeObject<Employee>(requestBody);
                employees = new Employees();
                employees.Add(emp);
            }
            return new OkObjectResult(employees);    
        }
    }

    public class Employee
    {
        [JsonProperty("Id")]
        public int Id { get; set; }
        [JsonProperty("Name")]
        public string Name { get; set; }
        [JsonProperty("DeptName")]
        public string DeptName { get; set; }
    }
    public class Employees : List<Employee>
    {
        public Employees()
        {
            Add(new Employee() {Id= 101,Name= "A",DeptName= "D1" });
            Add(new Employee() { Id = 102, Name = "B", DeptName = "D2" });
            Add(new Employee() { Id = 103, Name = "C", DeptName = "D1" });
            Add(new Employee() { Id = 104, Name = "D", DeptName = "D3" });
        }
    }

}
