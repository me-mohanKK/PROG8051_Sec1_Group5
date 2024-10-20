using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker_UnitTests
{
    public class UT_Groups
    {
        private GroupServices _groupService;

        [SetUp]
        public void Setup()
        {
            _groupService = new GroupServices();
        }

        [Test]
        public void AddGroup_ValidGroup_ShouldAddGroup()
        {
            // Arrange
            string groupName = "Group1";
            List<string> members = new List<string> { "John", "Jane" };

            // Act
            _groupService.AddGroup(groupName, members);

            // Assert
            Assert.That(_groupService.GroupExists(groupName), Is.True);
            Assert.That(_groupService.GetGroupMembers(groupName).Count, Is.EqualTo(2));
        }

        
        [Test]
        public void AddGroup_EmptyGroupName_ShouldThrowException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => _groupService.AddGroup("", new List<string> { "John" }));
            Assert.That(ex.Message, Is.EqualTo("Group name cannot be empty"));
        }

       

        [Test]
        public void EditGroup_ValidChanges_ShouldUpdateGroup()
        {
            // Arrange
            string groupName = "Group1";
            List<string> members = new List<string> { "John", "Jane" };
            _groupService.AddGroup(groupName, members);

            // Act
            _groupService.EditGroup(groupName, new List<string> { "John", "Mary" });

            // Assert
            var updatedMembers = _groupService.GetGroupMembers(groupName);
            Assert.That(updatedMembers.Contains("Mary"), Is.True);
            Assert.That(updatedMembers.Contains("Jane"), Is.False);
        }


        [Test]
        public void DeleteGroup_ValidGroup_ShouldRemoveGroup()
        {
            // Arrange
            string groupName = "Group1";
            List<string> members = new List<string> { "John", "Jane" };
            _groupService.AddGroup(groupName, members);

            // Act
            _groupService.DeleteGroup(groupName);

            // Assert
            Assert.That(_groupService.GroupExists(groupName), Is.False);
        }
    }
}
