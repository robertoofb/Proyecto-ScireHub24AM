﻿using Microsoft.EntityFrameworkCore;
using ProyectoWebDL.Context;
using ProyectoWebDL.Models.Entities;
using ProyectoWebDL.Services.IServices;

namespace ProyectoWebDL.Services.Service
{
    public class ArticuloServices : IArticuloServices
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHost;
        private readonly IHttpContextAccessor _httpContext;

        //Constructor para usar las tablas de base de datos
        public ArticuloServices(ApplicationDbContext context, IHttpContextAccessor httpContext, IWebHostEnvironment webHost)
        {
            _context = context;
            _httpContext = httpContext;
            _webHost = webHost;
        }

        public async Task<List<Articulo>> GetArticulos()
        {
            try
            {

                return await _context.Articulos.Include(x => x.Categorias).ToListAsync();

            }
            catch (Exception ex)
            {
                throw new Exception("Surgio un error" + ex.Message);
            }

        }

        public async Task<Articulo> GetByIdArticulo(int id)
        {
            try
            {
                //Articulo response = await _context.Articulos.FindAsync(id);

                Articulo response = await _context.Articulos.FirstOrDefaultAsync(x => x.PkArticulo == id);
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception("Surgio un error" + ex.Message);
            }

        }
        public async Task<Articulo> CrearArticulo(Articulo i)
        {
            try
            {
                var urlImagen = i.Img.FileName;
                i.UrlImagenPath = @"Img/articulos/" + urlImagen;
                var urlPdf = i.Pdf.FileName;
                i.UrlPdfPath = @"Pdf/articulos/" + urlPdf;

                Articulo request = new Articulo()
                {
                    Nombre = i.Nombre,
                    Descripcion = i.Descripcion,
                    Autor = i.Autor,
                    FkCategoria = i.FkCategoria,
                    UrlImagenPath = i.UrlImagenPath,
                    UrlPdfPath = i.UrlPdfPath,
                };
                SubirImg(urlImagen);
                SubirImg(urlPdf);

                var result = await _context.Articulos.AddAsync(request);
                _context.SaveChanges();

                return request;
            }
            catch (Exception ex)
            {
                throw new Exception("Surgio un error" + ex.Message);
            }
        }

        public async Task<Articulo> PublicarArticulo(Articulo i)
        {
            try
            {
                var urlImagen = i.Img.FileName;
                i.UrlImagenPath = @"Img/articulos/" + urlImagen;
                var urlPdf = i.Pdf.FileName;
                i.UrlPdfPath = @"Pdf/articulos/" + urlPdf;

                Articulo request = new Articulo()
                {
                    Nombre = i.Nombre,
                    Descripcion = i.Descripcion,
                    Autor = i.Autor,
                    FkCategoria = i.FkCategoria,
                    UrlImagenPath = i.UrlImagenPath,
                    UrlPdfPath = i.UrlPdfPath,
                };
                SubirImg(urlImagen);
                SubirImg(urlPdf);

                var result = await _context.Articulos.AddAsync(request);
                _context.SaveChanges();

                return request;
            }
            catch (Exception ex)
            {
                throw new Exception("Surgio un error" + ex.Message);
            }
        }

        public async Task<Articulo> EditarArticulo(Articulo i)
        {
            try
            {

                Articulo articulo = _context.Articulos.Find(i.PkArticulo);
                var urlImagen = i.Img.FileName;
                i.UrlImagenPath = @"Img/articulos/" + urlImagen;
                var urlPdf = i.Pdf.FileName;
                i.UrlPdfPath = @"Pdf/articulos/" + urlPdf;

                articulo.Nombre = i.Nombre;
                articulo.Descripcion = i.Descripcion;
                articulo.Autor = i.Autor;
                articulo.FkCategoria = i.FkCategoria;
                articulo.UrlImagenPath = i.UrlImagenPath;
                articulo.UrlPdfPath = i.UrlPdfPath;
                SubirImg(urlImagen);
                SubirImg(urlPdf);

                _context.Entry(articulo).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return articulo;

            }
            catch (Exception ex)
            {
                throw new Exception("Succedio un error " + ex.Message);
            }
        }
        public bool EliminarArticulo(int id)
        {
            try
            {
                Articulo articulo = _context.Articulos.Find(id);

                if (articulo != null)
                {
                    var res = _context.Articulos.Remove(articulo);
                    _context.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Surgió un error: " + ex.Message);
            }
        }

        public bool SubirImg(string Img)
        {
            bool res = false;

            try
            {
                string rutaprincipal = _webHost.WebRootPath;
                var archivos = _httpContext.HttpContext.Request.Form.Files;

                if (archivos.Count > 0 && !string.IsNullOrEmpty(archivos[0].FileName))
                {

                    var nombreArchivo = Img;
                    var subidas = Path.Combine(rutaprincipal, "Img", "articulos");

                    // Asegurarse de que el directorio de destino exista
                    if (!Directory.Exists(subidas))
                    {
                        Directory.CreateDirectory(subidas);
                    }

                    var rutaCompleta = Path.Combine(subidas, nombreArchivo);

                    using (var fileStream = new FileStream(rutaCompleta, FileMode.Create))
                    {
                        archivos[0].CopyTo(fileStream);
                        res = true;
                    }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error al subir la imagen: {ex.Message}");
            }

            return res;
        }
    }
}
