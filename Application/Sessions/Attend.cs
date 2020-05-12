using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Sessions {
    public class Attend {
        public class Command : IRequest {
            public Guid Id { get; set; }

        }

        public class Handler : IRequestHandler<Command> {
            private readonly DataContext _context;
            private readonly IUserAccessor _userAccessor;
            public Handler (DataContext context, IUserAccessor userAccessor) {
                _userAccessor = userAccessor;
                _context = context;
            }

            public async Task<Unit> Handle (Command request, CancellationToken cancellationToken)
            //this is a meditr unit, doeesn't return anything
            {
                var session = await _context.Sessions.FindAsync (request.Id);

                if (session == null)
                    throw new RestException (HttpStatusCode.NotFound, new { Session = "Could not find activity." });

                var user = await _context.Users.SingleOrDefaultAsync (x => x.UserName == _userAccessor.GetCurrentUsername ());
                var attendance = await _context.UserSessions.SingleOrDefaultAsync (x => x.SessionId == session.Id && x.AppUserId == user.Id);

                if (attendance != null)
                    throw new RestException (HttpStatusCode.BadRequest, new { Attendance = "Already attending this activity" });

                attendance = new UserSession {
                    Session = session,
                    AppUser = user,
                    IsHost = false,
                    DateJoined = DateTime.Now

                };

                _context.UserSessions.Add (attendance);

                var success = await _context.SaveChangesAsync () > 0;

                if (success) return Unit.Value;

                throw new Exception ("Problem saving changes.");

            }
        }

    }
}