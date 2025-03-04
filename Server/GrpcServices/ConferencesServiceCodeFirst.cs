﻿using AutoMapper;
using ConfTool.Server.Hubs;
using ConfTool.Server.Models;
using ConfTool.Shared.DTO;
using ConfTool.Shared.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ConfTool.Server.GrpcServices
{
    [Authorize]
    public class ConferencesServiceCodeFirst : IConferencesService
    {
        private readonly ConferencesDbContext _conferencesDbContext;
        private readonly IMapper _mapper;
        private readonly IHubContext<ConferencesHub> _hubContext;

        public ConferencesServiceCodeFirst(ConferencesDbContext conferencesDbContext, IMapper mapper, IHubContext<ConferencesHub> hubContext)
        {
            _conferencesDbContext = conferencesDbContext;
            _mapper = mapper;
            _hubContext = hubContext;
        }

        public async Task<ConferenceDetails> AddNewConferenceAsync(ConferenceDetails conference)
        {
            var conf = _mapper.Map<Conference>(conference);
            conf.DateCreated = DateTime.UtcNow;

            var entry = _conferencesDbContext.Conferences.Add(conf);
            await _conferencesDbContext.SaveChangesAsync();

            await _hubContext.Clients.All.SendAsync("NewConferenceAdded", entry.Entity.ID);

            return _mapper.Map<Shared.DTO.ConferenceDetails>(conf);

        }

        public async Task<ConferenceDetails> GetConferenceDetailsAsync(ConferenceDetailsRequest request)
        {
            var conferenceDetails = await _conferencesDbContext.Conferences.FindAsync(request.ID);

            if (conferenceDetails == null)
            {
                return null;
            }

            return _mapper.Map<Shared.DTO.ConferenceDetails>(conferenceDetails);
        }

        public async Task<IEnumerable<Shared.DTO.ConferenceOverview>> ListConferencesAsync()
        {
            var conferences = await _conferencesDbContext.Conferences.OrderByDescending(c => c.DateCreated).ToListAsync();
            var confs = _mapper.Map<IEnumerable<Shared.DTO.ConferenceOverview>>(conferences);

            return confs;
        }
    }
}
