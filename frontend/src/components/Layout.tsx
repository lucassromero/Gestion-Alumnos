import { Link, Outlet } from 'react-router-dom'

export function Layout() {
  return (
    <div className="min-h-screen flex flex-col bg-papel text-tinta">
      <header className="border-b border-borde bg-hoja">
        <div className="max-w-5xl mx-auto px-6 py-5">
          <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
            <Link to="/" className="font-serif text-2xl font-medium tracking-tight">
              Gestión de Profesoras Particulares
            </Link>
            <nav className="flex items-center gap-5">
              <Link
                to="/alumnos"
                className="text-sm font-medium text-tinta-suave hover:text-acento"
              >
                Alumnos
              </Link>
              <Link
                to="/materias"
                className="text-sm font-medium text-tinta-suave hover:text-acento"
              >
                Materias
              </Link>
            </nav>
          </div>
        </div>
      </header>

      <main className="flex-1 max-w-5xl w-full mx-auto px-6 py-8">
        <Outlet />
      </main>

      <footer className="border-t border-borde bg-hoja">
        <div className="max-w-5xl mx-auto px-6 py-4 text-xs text-tinta-suave">
          Cuaderno de trabajo para seguimiento académico.
        </div>
      </footer>
    </div>
  )
}
