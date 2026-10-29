import { Link } from 'react-router-dom'

export function NotFoundPage() {
  return (
    <div className="flex flex-col items-center justify-center py-20 text-center">
      <h1 className="font-serif text-5xl">404</h1>
      <p className="mt-2 text-tinta-suave">La página que buscás no existe.</p>
      <Link
        to="/alumnos"
        className="mt-6 rounded bg-acento px-4 py-2 text-sm font-medium text-hoja hover:bg-tinta"
      >
        Ir a Alumnos
      </Link>
    </div>
  )
}
