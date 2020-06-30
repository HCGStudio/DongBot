using System;
using System.Collections.Generic;
using System.Text;
using Autofac;

namespace HCGStudio.DongBot.Core.Service
{
    public interface ICustomServiceProvider
    {
        void UseCustomBuilder(ContainerBuilder builder);
    }
}
