using LabProject.Models;

namespace LabProject.Helpers
{
    public static class FakeDataHelper
    {
        public static List<Class> GenerateFakeData()
        {
            var list = new List<Class>();
            for (int i = 1; i <= 100; i++)
            {
                list.Add(new Class
                {
                    Name = $"Class {i}",
                    PersonCount = i % 30 + 1,
                    Description = $"Description {i}",
                    IsActive = true
                });
            }
            return list;
        }
    }
}
