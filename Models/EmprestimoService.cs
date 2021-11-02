using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections;

namespace Biblioteca.Models
{
    public class EmprestimoService 
    {
        public void Inserir(Emprestimo e)
        {
            using(BibliotecaContext bc = new BibliotecaContext())
            {
                bc.Emprestimos.Add(e);
                bc.SaveChanges();
            }
        }

        public void Atualizar(Emprestimo e)
        {
            using(BibliotecaContext bc = new BibliotecaContext())
            {
                Emprestimo emprestimo = bc.Emprestimos.Find(e.Id);
                emprestimo.NomeUsuario = e.NomeUsuario;
                emprestimo.Telefone = e.Telefone;
                emprestimo.LivroId = e.LivroId;
                emprestimo.DataEmprestimo = e.DataEmprestimo;
                emprestimo.DataDevolucao = e.DataDevolucao;

                bc.SaveChanges();
            }
        }

        public ICollection<Emprestimo> ListarTodos(FiltrosEmprestimos filtro = null)
        {
            using(BibliotecaContext bc = new BibliotecaContext())
            {
                IQueryable<Emprestimo> query;
                if(filtro !=null)
                {
                    //definindo dinamicamente a filtragem
                    switch(filtro.TipoFiltro)
                    {
                        case "Usuario":
                            query = bc.Emprestimos.Where(l => l.NomeUsuario.Contains(filtro.Filtro));
                        break;

                        case "Livro":
                        List<Livro> LivrosFiltro = bc.Livros.Where(l => l.Titulo.Contains(filtro.Filtro)).ToList();

                           List<int> LivrosId = new List<int>();
                            for(int book = 0; book <LivrosFiltro.Count; book++){
                                LivrosId.Add(LivrosFiltro[book].Id);
                            }
                            query = bc.Emprestimos.Where( l => LivrosId.Contains(l.LivroId));
                            var debug = query.ToList();
                            break;

                            default:
                            query = bc.Emprestimos;
                            break;

                        
                    }
                }
                else
                {
                    // caso filtro não tenha sido informado
                    query = bc.Emprestimos;
                }
                List<Emprestimo> ListaBusca = query.OrderByDescending(l => l.DataEmprestimo).ToList();
                   for(int book = 0; book < ListaBusca.Count; book++){
                       ListaBusca[book].Livro = bc.Livros.Find(ListaBusca[book].LivroId);
                   }

                //ordenação padrão
                return ListaBusca;
            }
        }

        public Emprestimo ObterPorId(int id)
        {
            using(BibliotecaContext bc = new BibliotecaContext())
            {
                return bc.Emprestimos.Find(id);
            }
        }
    }
}