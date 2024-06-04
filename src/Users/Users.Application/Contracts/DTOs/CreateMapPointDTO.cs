﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Users.Application.Contracts.DTOs
{
    public class CreateMapPointDTO
    {
        public float[] Position { get; set; }

        public string Title { get; set; }

        public string? Description { get; set; }
    }
}
