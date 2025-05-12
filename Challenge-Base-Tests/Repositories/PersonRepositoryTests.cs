using NUnit.Framework;
using Challenge_Base.Data;
using Challenge_Base.Data.Repositories;
using Challenge_Base.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace person_wpf_demo_tests
{
    public class PersonRepositoryTests
    {
        private ApplicationDbContext _dbContext;
        private PersonRepository _repository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _dbContext = new ApplicationDbContext(options);
            _dbContext.Database.EnsureCreated();
            _repository = new PersonRepository(_dbContext);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [Test]
        public void Saving_a_valid_person_adds_person_to_database()
        {
            var person = new Person { Firstname = "John", Lastname = "Doe", DateOfBirth = new DateTime(1990, 1, 1) };

            _repository.Save(person);

            var savedPerson = _dbContext.Persons.FirstOrDefault(
                p => p.Firstname == "John" && p.Lastname == "Doe");
            Assert.That(savedPerson, Is.Not.Null);
        }

        [Test]
        public void Getting_all_persons_returns_all_persons_from_database()
        {
            var persons = new List<Person>
            {
                new Person { Firstname = "John", Lastname = "Doe", DateOfBirth = new DateTime(1990, 1, 1) },
                new Person { Firstname = "Jane", Lastname = "Doe", DateOfBirth = new DateTime(1992, 2, 2) }
            };
            _dbContext.Persons.AddRange(persons);
            _dbContext.SaveChanges();

            var result = _repository.GetAll();

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].Firstname, Is.EqualTo("John"));
            Assert.That(result[1].Firstname, Is.EqualTo("Jane"));
        }

        [Test]
        public void Updating_a_valid_person_updates_person_in_database()
        {
            var person = new Person { Firstname = "John", Lastname = "Doe", DateOfBirth = new DateTime(1990, 1, 1) };
            _repository.Save(person);
            _dbContext.SaveChanges();

            person.Firstname = "Johnny";
            _repository.Update(person);
            _dbContext.SaveChanges();

            var updatedPerson = _dbContext.Persons.FirstOrDefault(p => p.Id == person.Id);
            Assert.That(updatedPerson.Firstname, Is.EqualTo("Johnny"));
        }

        [Test]
        public void Deleting_a_valid_person_removes_person_from_database()
        {
            var person = new Person { Firstname = "John", Lastname = "Doe", DateOfBirth = new DateTime(1990, 1, 1) };
            _dbContext.Persons.Add(person);
            _dbContext.SaveChanges();

            _repository.Delete(person);
            _dbContext.SaveChanges();

            var deletedPerson = _dbContext.Persons.FirstOrDefault(p => p.Id == person.Id);
            Assert.That(deletedPerson, Is.Null);
        }
    }
}




