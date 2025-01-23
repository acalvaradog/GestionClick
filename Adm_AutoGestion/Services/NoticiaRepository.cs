using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Adm_AutoGestion.Models;
using System.IO;
using Autogestion.Shared.DTO.Noticia;

namespace Adm_AutoGestion.Services
{
    public class NoticiaRepository
    {

        AutogestionContext _db = new AutogestionContext();
        public async Task<List<Noticia>> ObtenerTodos()
        {
            try
            {

            var noticias =  _db.Noticias.ToList();
                return noticias;
            }
            catch (Exception ex)
            {

                throw;
            }
         
        }
        public async Task<List<Noticia>> ObtenerTodosActuales()
        {
            try
            {

                var noticias = _db.Noticias.Where(x => x.Activo == true && x.Publicacion <= DateTime.Now).ToList();
                return noticias;
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public async Task<Noticia> ObtenerNoticia(int id)
        {
            try
            {

                var noticias = _db.Noticias.Find(id);
                return noticias;
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        internal void Crear(Noticia model)
        {

            using (var db = new AutogestionContext()) {
                db.Noticias.Add(model);
                db.SaveChanges();
            
            }
          
        }

        public Noticia CrearNoticia(Noticia noticia, IEnumerable<HttpPostedFileBase> imagenes, string rutaCarpeta)
        {
            // Guardar la noticia
            _db.Noticias.Add(noticia);
            _db.SaveChanges(); // Asegúrate de guardar primero la noticia para obtener su ID.

            // Procesar las imágenes después de que la noticia tenga un ID
            if (imagenes != null && imagenes.Any())
            {
                foreach (var imagen in imagenes)
                {
                    if (imagen != null)
                    { 
                        if (imagen.ContentLength > 0)
                        {
                            // Obtener el nombre del archivo y la ruta completa donde se guardará
                            string nombreArchivo = Path.GetFileName(imagen.FileName);
                            string rutaCompleta = Path.Combine(rutaCarpeta, nombreArchivo);

                            // Guardar la imagen en el servidor
                            imagen.SaveAs(rutaCompleta);

                            // Crear el registro de la imagen en la base de datos y asociarlo a la noticia
                            var noticiaImagen = new NoticiaImagen
                            {
                                ImagenUrl = "/ImagenesNoticias/" + nombreArchivo,
                                NoticiaId = noticia.Id // Asociamos la imagen a la noticia ya guardada
                            };

                            // Guardar la imagen en la base de datos
                            _db.NoticiaImagen.Add(noticiaImagen);
                        }
                }
                }
                // Guardar los registros de las imágenes en la base de datos
                _db.SaveChanges();
            }

            return noticia;
        }

        public void ActualizarNoticia(Noticia noticia, IEnumerable<HttpPostedFileBase> imagenes, string rutaCarpeta)
        {
            var noticiaExistente = _db.Noticias.FirstOrDefault(n => n.Id == noticia.Id);
            if (noticiaExistente != null)
            {
                noticiaExistente.Titulo = noticia.Titulo;
                noticiaExistente.Contenido = noticia.Contenido;
                noticiaExistente.Autor = noticia.Autor;
               // noticiaExistente.Publicacion = noticia.Publicacion;
                noticiaExistente.Activo = noticia.Activo;

                _db.SaveChanges();

                // Procesar las nuevas imágenes
                if (imagenes != null && imagenes.Any())
                {
                    foreach (var imagen in imagenes)
                    {
                        if (imagen != null)
                        {
                            if (imagen.ContentLength > 0)
                            {
                                string nombreArchivo = Path.GetFileName(imagen.FileName);
                                string rutaCompleta = Path.Combine(rutaCarpeta, nombreArchivo);

                                imagen.SaveAs(rutaCompleta);

                                var noticiaImagen = new NoticiaImagen
                                {
                                    ImagenUrl = "/ImagenesNoticias/" + nombreArchivo,
                                    NoticiaId = noticiaExistente.Id
                                };
                                _db.NoticiaImagen.Add(noticiaImagen);
                            }
                        }
                    }
                    _db.SaveChanges();
                }
            }
        }


        public async Task<List<NoticiaDTO>> ObtenerNoticiasConImagenesAsync()
        {
            try
            {
                // Obtener todas las noticias activas
                var noticias = await _db.Noticias
                    .Where(n => n.Activo) // Solo noticias activas
                    .ToListAsync();

                if (!noticias.Any())
                    return new List<NoticiaDTO>(); // Retornar una lista vacía si no hay noticias activas

                // Extraer los IDs de las noticias
                var noticiaIds = noticias.Select(n => n.Id).ToList();

                // Obtener las imágenes asociadas a los IDs de noticias
                var imagenes = await _db.NoticiaImagen
                    .Where(ni => noticiaIds.Contains(ni.NoticiaId)) // Filtrar imágenes por ID de noticia
                    .ToListAsync();

                // Mapear noticias a NoticiaDTO
                var noticiaDtos = noticias.Select(n => new NoticiaDTO
                {
                    Id = n.Id,
                    Titulo = n.Titulo,
                    Contenido = n.Contenido,
                    Autor = n.Autor,
                    Publicacion = n.Publicacion,
                    Activo = n.Activo,
                    EmpleadoId = n.EmpleadoId,
                    //Empleado = n.Empleado != null ? new EmpleadoDTO
                    //{
                    //    Id = n.Empleado.Id,
                    //    Nombre = n.Empleado.Nombre,
                    //    // Agregar otras propiedades necesarias
                    //} : null,
                    Imagenes = imagenes
                        .Where(i => i.NoticiaId == n.Id)
                        .Select(i => new NoticiaImagenesDTO
                        {
                            Id = i.Id,
                            ImagenUrl = i.ImagenUrl
                        })
                        .ToList()
                }).ToList();

                return noticiaDtos;
            }
            catch (Exception ex)
            {
                // Opcional: registrar el error para diagnóstico
                Console.WriteLine($"Error al obtener noticias con imágenes: {ex.Message}");
                throw new Exception("Ocurrió un error al obtener las noticias.", ex);
            }
        }
    }
}