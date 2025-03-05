using System;
using System.Collections.Generic;

namespace Data;

public partial class tblUser
{
    public int Id { get; set; }

    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;
}
