// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2016/05/27</date>

namespace PetaPoco.Tests.Integration.Models
{
    public enum OrderLineStatus : byte
    {
        Allocated,
        Backorder,
        Pending
    }
}