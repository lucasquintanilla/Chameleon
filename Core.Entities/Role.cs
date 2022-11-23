﻿using Microsoft.AspNetCore.Identity;
using System;

namespace Core.Entities
{
    public enum RoleType { Administrator, Moderator, Anonymous }

    public class Role : IdentityRole<Guid> { }
}
