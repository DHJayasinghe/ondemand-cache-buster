CREATE TABLE [dbo].[ServiceRegistry](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AppName] [varchar](100) NOT NULL,
	[HostName] [varchar](250) NOT NULL,
	[Port] [int] NOT NULL,
	[IP] [varchar](100) NOT NULL,
	[Region] [varchar](100) NULL,
	[RegisteredDateTime] [datetime2](7) NOT NULL,
	[LastActiveDateTime] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_ServiceRegistry] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
