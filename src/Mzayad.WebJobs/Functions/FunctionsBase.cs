﻿using System.Configuration;
using Mzayad.Data;
using Mzayad.Services;
using Mzayad.Services.Identity;
using Mzayad.Services.Queues;

namespace Mzayad.WebJobs.Functions
{
    public abstract class FunctionsBase
    {
        protected IDataContextFactory DataContextFactory { get; }
        protected IQueueService QueueService { get; }

        private UserService _userService;
        protected UserService UserService => _userService ?? (_userService = new UserService(DataContextFactory));

        private BidService _bidService;
        protected BidService BidService => _bidService ?? (_bidService = new BidService(DataContextFactory, QueueService));

        private TrophyService _trophyService;
        protected TrophyService TrophyService => _trophyService ?? (_trophyService = new TrophyService(DataContextFactory));

        protected FunctionsBase()
        {
            DataContextFactory = new DataContextFactory();
            QueueService = new QueueService(ConfigurationManager.ConnectionStrings["QueueConnection"].ConnectionString);
        }
    }
}