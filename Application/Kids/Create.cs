using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using AutoMapper;
using Domain;
// using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Kids {
    public class Create {
        public class Command : IRequest {
            public Guid Id { get; set; }
            //Guid allows to create id from service side or client side
            public string Name { get; set; }
            public string Age { get; set; }
            public string Interests { get; set; }
            public string WorkingOn { get; set; }

        }

        public class Handler : IRequestHandler<Command> {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            private readonly IUserAccessor _userAccessor;
            public Handler (DataContext context, IMapper mapper, IUserAccessor userAccessor) {
                _mapper = mapper;
                _userAccessor = userAccessor;
                _context = context;
            }

            public async Task<Unit> Handle (Command request, CancellationToken cancellationToken)
            //this is a meditr unit, doeesn't return anything
            {
                {
                    var kid = new Kid {
                        Id = request.Id,
                        Name = request.Name,
                        Age = request.Age,
                        Interests = request.Interests,
                        WorkingOn = request.WorkingOn

                    };
                    _context.Kids.Add (kid);

                    var user = await _context.Users.SingleOrDefaultAsync (x => x.UserName == _userAccessor.GetCurrentUsername ());

                    var parent = new UserKid {
                        AppUser = user,
                        Kid = kid,
                        IsParent = true

                    };

                    _context.UserKids.Add (parent);

                    var success = await
                    _context.SaveChangesAsync () > 0;

                    if (success) return Unit.Value;

                    throw new Exception ("Problem saving changes.");

                }
            }
        }
    }
}