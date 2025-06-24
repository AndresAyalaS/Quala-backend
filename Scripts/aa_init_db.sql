-- ============================================
-- Scripts de Base de Datos para QUALA
-- ============================================

-- 1. CREACIÓN DE TABLAS
-- ============================================

-- Tabla de Monedas
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[aa_mon_moneda]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[aa_mon_moneda] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [Codigo] NVARCHAR(10) NOT NULL UNIQUE,
        [Nombre] NVARCHAR(100) NOT NULL,
        [Simbolo] NVARCHAR(5) NOT NULL,
        [Activo] BIT NOT NULL DEFAULT 1,
        [FechaCreacion] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [FechaModificacion] DATETIME2 NOT NULL DEFAULT GETDATE()
    );
END

-- Tabla de Usuarios
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[aa_usr_usuario]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[aa_usr_usuario] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [NombreUsuario] NVARCHAR(50) NOT NULL UNIQUE,
        [Email] NVARCHAR(150) NOT NULL UNIQUE,
        [PasswordHash] NVARCHAR(100) NOT NULL,
        [Activo] BIT NOT NULL DEFAULT 1,
        [FechaCreacion] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [FechaModificacion] DATETIME2 NOT NULL DEFAULT GETDATE()
    );
END

-- Tabla de Sucursales
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[aa_suc_sucursal]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[aa_suc_sucursal] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [Codigo] INT NOT NULL UNIQUE,
        [Descripcion] NVARCHAR(250) NOT NULL,
        [Direccion] NVARCHAR(250) NOT NULL,
        [Identificacion] NVARCHAR(50) NOT NULL,
        [FechaCreacion] DATETIME2 NOT NULL,
        [MonedaId] INT NOT NULL,
        [Activo] BIT NOT NULL DEFAULT 1,
        [FechaModificacion] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [FK_aa_suc_sucursal_MonedaId] FOREIGN KEY ([MonedaId]) 
            REFERENCES [dbo].[aa_mon_moneda] ([Id])
    );
END

-- 2. DATOS INICIALES
-- ============================================

-- Insertar monedas predeterminadas
IF NOT EXISTS (SELECT 1 FROM [dbo].[aa_mon_moneda])
BEGIN
    INSERT INTO [dbo].[aa_mon_moneda] ([Codigo], [Nombre], [Simbolo])
    VALUES 
        ('COP', 'Peso Colombiano', '$'),
        ('USD', 'Dólar Americano', 'US$'),
        ('EUR', 'Euro', '€'),
        ('MXN', 'Peso Mexicano', 'MX$'),
        ('PEN', 'Sol Peruano', 'S/');
END

-- Insertar usuario de prueba (password: Admin123!)
IF NOT EXISTS (SELECT 1 FROM [dbo].[aa_usr_usuario])
BEGIN
    INSERT INTO [dbo].[aa_usr_usuario] ([NombreUsuario], [Email], [PasswordHash])
    VALUES ('admin', 'admin@quala.com', '123456');
END

-- 3. PROCEDIMIENTOS ALMACENADOS - SUCURSALES
-- ============================================

-- Obtener todas las sucursales
CREATE OR ALTER PROCEDURE [dbo].[aa_sp_sucursal_obtener_todas]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        s.[Id],
        s.[Codigo],
        s.[Descripcion],
        s.[Direccion],
        s.[Identificacion],
        s.[FechaCreacion],
        s.[MonedaId],
        m.[Nombre] AS MonedaNombre,
        s.[Activo],
        s.[FechaModificacion]
    FROM [dbo].[aa_suc_sucursal] s
    INNER JOIN [dbo].[aa_mon_moneda] m ON s.[MonedaId] = m.[Id]
    WHERE s.[Activo] = 1
    ORDER BY s.[Codigo];
END

-- Obtener sucursal por ID
CREATE OR ALTER PROCEDURE [dbo].[aa_sp_sucursal_obtener_por_id]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        s.[Id],
        s.[Codigo],
        s.[Descripcion],
        s.[Direccion],
        s.[Identificacion],
        s.[FechaCreacion],
        s.[MonedaId],
        m.[Nombre] AS MonedaNombre,
        s.[Activo],
        s.[FechaModificacion]
    FROM [dbo].[aa_suc_sucursal] s
    INNER JOIN [dbo].[aa_mon_moneda] m ON s.[MonedaId] = m.[Id]
    WHERE s.[Id] = @Id AND s.[Activo] = 1;
END

-- Crear sucursal
CREATE OR ALTER PROCEDURE [dbo].[aa_sp_sucursal_crear]
    @Codigo INT,
    @Descripcion NVARCHAR(250),
    @Direccion NVARCHAR(250),
    @Identificacion NVARCHAR(50),
    @FechaCreacion DATETIME2,
    @MonedaId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Validar que no exista el código
    IF EXISTS (SELECT 1 FROM [dbo].[aa_suc_sucursal] WHERE [Codigo] = @Codigo AND [Activo] = 1)
    BEGIN
        RAISERROR('Ya existe una sucursal con el código especificado', 16, 1);
        RETURN;
    END
    
    -- Validar fecha de creación
    IF @FechaCreacion < CAST(GETDATE() AS DATE)
    BEGIN
        RAISERROR('La fecha de creación no puede ser anterior a la fecha actual', 16, 1);
        RETURN;
    END
    
    -- Validar que existe la moneda
    IF NOT EXISTS (SELECT 1 FROM [dbo].[aa_mon_moneda] WHERE [Id] = @MonedaId AND [Activo] = 1)
    BEGIN
        RAISERROR('La moneda especificada no existe', 16, 1);
        RETURN;
    END
    
    DECLARE @Id INT;
    
    INSERT INTO [dbo].[aa_suc_sucursal] 
    (
        [Codigo], 
        [Descripcion], 
        [Direccion], 
        [Identificacion], 
        [FechaCreacion], 
        [MonedaId]
    )
    VALUES 
    (
        @Codigo, 
        @Descripcion, 
        @Direccion, 
        @Identificacion, 
        @FechaCreacion, 
        @MonedaId
    );
    
    SET @Id = SCOPE_IDENTITY();
    
    -- Retornar la sucursal creada
    EXEC [dbo].[aa_sp_sucursal_obtener_por_id] @Id;
END

-- Actualizar sucursal
CREATE OR ALTER PROCEDURE [dbo].[aa_sp_sucursal_actualizar]
    @Id INT,
    @Codigo INT,
    @Descripcion NVARCHAR(250),
    @Direccion NVARCHAR(250),
    @Identificacion NVARCHAR(50),
    @MonedaId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Validar que existe la sucursal
    IF NOT EXISTS (SELECT 1 FROM [dbo].[aa_suc_sucursal] WHERE [Id] = @Id AND [Activo] = 1)
    BEGIN
        RAISERROR('La sucursal especificada no existe', 16, 1);
        RETURN;
    END
    
    -- Validar que no exista otro registro con el mismo código
    IF EXISTS (SELECT 1 FROM [dbo].[aa_suc_sucursal] WHERE [Codigo] = @Codigo AND [Id] != @Id AND [Activo] = 1)
    BEGIN
        RAISERROR('Ya existe otra sucursal con el código especificado', 16, 1);
        RETURN;
    END
    
    -- Validar que existe la moneda
    IF NOT EXISTS (SELECT 1 FROM [dbo].[aa_mon_moneda] WHERE [Id] = @MonedaId AND [Activo] = 1)
    BEGIN
        RAISERROR('La moneda especificada no existe', 16, 1);
        RETURN;
    END
    
    UPDATE [dbo].[aa_suc_sucursal]
    SET 
        [Codigo] = @Codigo,
        [Descripcion] = @Descripcion,
        [Direccion] = @Direccion,
        [Identificacion] = @Identificacion,
        [MonedaId] = @MonedaId,
        [FechaModificacion] = GETDATE()
    WHERE [Id] = @Id;
    
    -- Retornar la sucursal actualizada
    EXEC [dbo].[aa_sp_sucursal_obtener_por_id] @Id;
END

-- Eliminar sucursal (eliminación lógica)
CREATE OR ALTER PROCEDURE [dbo].[aa_sp_sucursal_eliminar]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Validar que existe la sucursal
    IF NOT EXISTS (SELECT 1 FROM [dbo].[aa_suc_sucursal] WHERE [Id] = @Id AND [Activo] = 1)
    BEGIN
        RAISERROR('La sucursal especificada no existe', 16, 1);
        RETURN;
    END
    
    UPDATE [dbo].[aa_suc_sucursal]
    SET 
        [Activo] = 0,
        [FechaModificacion] = GETDATE()
    WHERE [Id] = @Id;
    
    SELECT 1 AS Success;
END

-- 4. PROCEDIMIENTOS ALMACENADOS - MONEDAS
-- ============================================

-- Obtener todas las monedas activas
CREATE OR ALTER PROCEDURE [dbo].[aa_sp_moneda_obtener_todas]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        [Id],
        [Codigo],
        [Nombre],
        [Simbolo],
        [Activo]
    FROM [dbo].[aa_mon_moneda]
    WHERE [Activo] = 1
    ORDER BY [Nombre];
END

-- 5. PROCEDIMIENTOS ALMACENADOS - AUTENTICACIÓN
-- ============================================

-- Validar usuario
CREATE OR ALTER PROCEDURE [dbo].[aa_sp_usuario_validar]
    @NombreUsuario NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        [Id],
        [NombreUsuario],
        [Email],
        [PasswordHash],
        [Activo]
    FROM [dbo].[aa_usr_usuario]
    WHERE [NombreUsuario] = @NombreUsuario AND [Activo] = 1;
END