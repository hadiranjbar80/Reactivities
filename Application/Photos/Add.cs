using Application.Core;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Photos
{
    public class Add
    {
        public class Command : IRequest<Result<Photo>>
        {
            public IFormFile File { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Photo>>
        {
            private readonly IUserAccessor _userAccessor;
            private readonly DataContext _context;

            public Handler(DataContext context, IUserAccessor userAccessor)
            {
                _context = context;
                _userAccessor = userAccessor;

            }

            public async Task<Result<Photo>> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _context.Users.Include(p => p.Photos)
                    .FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());

                // Saving photo
                if (request.File.Length > 0)
                {
                    var photoId = Guid.NewGuid().ToString();
                    var fileExtension = request.File.ContentType.Split("/").Last();
                    var photoName = $"{photoId}.{fileExtension}";
                    

                    string photoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", "Photos", photoName);

                    using (var stream = new FileStream(photoPath, FileMode.Create))
                    {
                        request.File.CopyTo(stream);
                    };
                    
                    var photo = new Photo
                    {
                        Id = photoId,
                        PhotoName = photoName,
                    };

                    if (!user.Photos.Any(x => x.IsMain)) photo.IsMain = true;

                    user.Photos.Add(photo);

                    var result = await _context.SaveChangesAsync() > 0;

                    if (result) return Result<Photo>.Success(photo);
                }

                return Result<Photo>.Failure("Problem adding photo");
            }
        }
    }
}