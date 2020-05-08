// using System;
// using System.Linq;
// using System.Net;
// using System.Threading;
// using System.Threading.Tasks;
// using Application.Errors;
// using Application.Interfaces;
// using Application.Validators;
// using Domain;
// using FluentValidation;
// using MediatR;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.EntityFrameworkCore;
// using Persistence;

// namespace Application.User {
//     public class Register {
//         public class Command : IRequest<User> {
//             public string DisplayName { get; set; }
//             public string Username { get; set; }
//             public string Email { get; set; }
//             public string Password { get; set; }
//             public string Address { get; set; }
//             public string City { get; set; }
//             public string State { get; set; }
//             public string Interests { get; set; }
//             public string BgCheck { get; set; }
//             public string FirstAid { get; set; }
//             public string Terms { get; set; }
//             public string Token { get; set; }
//             public string Image { get; set; }

//         }

//         public class CommandValidator : AbstractValidator<Command> {
//             public CommandValidator () {
//                 RuleFor (x => x.DisplayName).NotEmpty ();
//                 RuleFor (x => x.Username).NotEmpty ();
//                 RuleFor (x => x.Email).NotEmpty ().EmailAddress ();
//                 RuleFor (x => x.Password).Password ();
//             }
//         }

//         public class Handler : IRequestHandler<Command, User> {
//             private readonly DataContext _context;
//             private readonly UserManager<AppUser> _userManager;
//             private readonly IJwtGenerator _jwtGenerator;

//             public Handler (DataContext context, UserManager<AppUser> userManager, IJwtGenerator jwtGenerator) {
//                 _context = context;
//                 _userManager = userManager;
//                 _jwtGenerator = jwtGenerator;
//             }

//             public async Task<User> Handle (Command request, CancellationToken cancellationToken)
//             //this is a meditr unit,returns User
//             {
//                 if (await _context.Users.Where (x => x.Email == request.Email).AnyAsync ())
//                     throw new RestException (HttpStatusCode.BadRequest, new { Email = "Email already exists" });

//                 if (await _context.Users.Where (x => x.UserName == request.Username).AnyAsync ())
//                     throw new RestException (HttpStatusCode.BadRequest, new { Username = "Username already exists" });

//             var user = new AppUser {
//                 DisplayName = request.DisplayName,
//                 Email = request.Email,
//                 UserName = request.Username
//             };

//             var result = await _userManager.CreateAsync (user, request.Password);
//             if (result.Succeeded) {
//                 return new User {
//                     DisplayName = user.DisplayName,
//                         Token = _jwtGenerator.CreateToken (user),
//                         Username = user.UserName,
//                         Image = null
//                 };
//             }

//             throw new Exception ("Problem creating user.");

//         }
//     }
// }
// }