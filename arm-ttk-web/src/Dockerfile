#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
#Add POWESHELL
RUN apt-get update
RUN apt-get install -y wget apt-transport-https software-properties-common git
RUN wget -q https://packages.microsoft.com/config/ubuntu/22.10/packages-microsoft-prod.deb
RUN wget -q https://github.com/PowerShell/PowerShell/releases/download/v7.3.2/powershell_7.3.2-1.deb_amd64.deb
RUN dpkg -i powershell_7.3.2-1.deb_amd64.deb
RUN apt-get install -f
#Clone ARM_TTK
WORKDIR /app
RUN git clone https://github.com/Azure/arm-ttk.git 
RUN chmod +x /app/arm-ttk/arm-ttk/Test-AzTemplate.sh
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ArmValidation.csproj", "."]
RUN dotnet restore "./ArmValidation.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "ArmValidation.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ArmValidation.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ArmValidation.dll"]