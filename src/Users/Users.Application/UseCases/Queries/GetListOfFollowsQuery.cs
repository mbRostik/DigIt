﻿using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Users.Application.Contracts.DTOs;
using Users.Domain.Entities;

namespace Users.Application.UseCases.Queries
{
    public record GetListOfFollowsQuery(string userId) : IRequest<List<Follow>>;

}
