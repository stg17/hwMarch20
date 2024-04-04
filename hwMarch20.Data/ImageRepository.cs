using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hwMarch20.Data
{
    public class ImageRepository
    {
        private readonly string _connectionString;
        public ImageRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Image> GetImages()
        {
            using var context = new ImageDataContext(_connectionString);
            return context.Images.ToList();
        }

        public void AddImage(Image image)
        {
            using var context = new ImageDataContext(_connectionString);
            context.Images.Add(image);
            context.SaveChanges();
        }

        public Image GetById(int id)
        {
            using var context = new ImageDataContext(_connectionString);
            return context.Images.FirstOrDefault(i => i.Id == id);
        }

        public void AddLike(int id)
        {
            using var context = new ImageDataContext(_connectionString);
            context.Images.FirstOrDefault(i => i.Id == id).Likes += 1;
            context.SaveChanges();
        }

        public int UpdateLikes(int id)
        {
            using var context = new ImageDataContext(_connectionString);
            return context.Images.FirstOrDefault(i => i.Id == id).Likes;
        }
    }
}
