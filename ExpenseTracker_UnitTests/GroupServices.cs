using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker_UnitTests
{
    public class GroupServices
    {
        private List<Group> _groups = new List<Group>();

        // Method to create a new group
        public void AddGroup(string groupName, List<string> members)
        {
            if (string.IsNullOrWhiteSpace(groupName))
            {
                throw new ArgumentException("Group name cannot be empty");
            }

            if (_groups.Any(g => g.GroupName.Equals(groupName, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ArgumentException("Group name already exists");
            }

            if (members == null || members.Count == 0)
            {
                throw new ArgumentException("Group must have at least one member");
            }

            var group = new Group
            {
                GroupName = groupName,
                Members = members
            };

            _groups.Add(group);
        }

        // Method to edit an existing group
        public void EditGroup(string groupName, List<string> newMembers)
        {
            var group = _groups.FirstOrDefault(g => g.GroupName.Equals(groupName, StringComparison.OrdinalIgnoreCase));

            if (group == null)
            {
                throw new ArgumentException("Group does not exist");
            }

            if (newMembers == null || newMembers.Count == 0)
            {
                throw new ArgumentException("Group must have at least one member");
            }

            group.Members = newMembers;
        }

        // Method to delete a group
        public void DeleteGroup(string groupName)
        {
            var group = _groups.FirstOrDefault(g => g.GroupName.Equals(groupName, StringComparison.OrdinalIgnoreCase));

            if (group == null)
            {
                throw new ArgumentException("Group does not exist");
            }

            _groups.Remove(group);
        }

        // Helper method to check if a group exists (for testing purposes)
        public bool GroupExists(string groupName)
        {
            return _groups.Any(g => g.GroupName.Equals(groupName, StringComparison.OrdinalIgnoreCase));
        }

        // Helper method to get group members (for testing purposes)
        public List<string> GetGroupMembers(string groupName)
        {
            var group = _groups.FirstOrDefault(g => g.GroupName.Equals(groupName, StringComparison.OrdinalIgnoreCase));
            return group?.Members;
        }
    }

    // Group class to represent the group and its members
    public class Group
    {
        public string GroupName { get; set; }
        public List<string> Members { get; set; }
    }
}

