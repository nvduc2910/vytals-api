
using AutoMapper_Demo.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vytals.Infrastructures.Mappings;

namespace AutoMapper_Demo.ViewModels
{
    public class StudentViewModel : IMapFrom<Student>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Age { get; set; }
    }
}
