﻿// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by CashOverflow Team
// --------------------------------------------------------

using System.Threading.Tasks;
using CashOverflow.Brokers.Loggings;
using CashOverflow.Brokers.Storages;
using CashOverflow.Models.Companies;

namespace CashOverflow.Services.Foundations.Companies
{
    public class CompanyService : ICompanyService
    {
        private readonly IStorageBroker storageBroker;
        private readonly ILoggingBroker loggingBroker;

        public CompanyService(IStorageBroker storageBroker, ILoggingBroker loggingBroker)
        {
			this.storageBroker = storageBroker;
            this.loggingBroker = loggingBroker;
		}

        public async ValueTask<Company> AddCompanyAsync(Company company) =>
             await this.storageBroker.InsertCompanyAsync(company);
    }
}
