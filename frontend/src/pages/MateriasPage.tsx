import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { EmptyState } from '../components/EmptyState'
import { ErrorMessage } from '../components/ErrorMessage'
import { Loading } from '../components/Loading'
import { cambiarEstadoMateria, listarMaterias } from '../services/api'
import type { Materia } from '../types/materia'

export function MateriasPage() {
  const [materias, setMaterias] = useState<Materia[]>([])
  const [cargando, setCargando] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [filtroActiva, setFiltroActiva] = useState<boolean | undefined>(undefined)
  const [buscar, setBuscar] = useState('')

  async function cargar() {
    setCargando(true)
    setError(null)
    try {
      const data = await listarMaterias({
        activa: filtroActiva,
        buscar: buscar.trim() || undefined,
      })
      setMaterias(data)
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Error desconocido')
    } finally {
      setCargando(false)
    }
  }

  useEffect(() => {
    cargar()
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [filtroActiva])

  useEffect(() => {
    const timeout = setTimeout(() => {
      cargar()
    }, 300)
    return () => clearTimeout(timeout)
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [buscar])

  async function toggleEstado(materia: Materia) {
    try {
      const actualizada = await cambiarEstadoMateria(materia.id, !materia.activa)
      setMaterias((prev) =>
        prev.map((m) => (m.id === actualizada.id ? actualizada : m))
      )
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Error al cambiar el estado')
    }
  }

  return (
    <div className="space-y-6">
      <div className="flex flex-col gap-4 sm:flex-row sm:items-end sm:justify-between">
        <div>
          <h1 className="font-serif text-3xl">Materias</h1>
          <p className="mt-1 text-sm text-tinta-suave">
            {materias.length} {materias.length === 1 ? 'materia' : 'materias'}
          </p>
        </div>
        <Link
          to="/materias/nueva"
          className="inline-flex justify-center rounded bg-acento px-4 py-2 text-sm font-medium text-hoja hover:bg-tinta"
        >
          Nueva materia
        </Link>
      </div>

      <div className="flex flex-col gap-3 sm:flex-row">
        <input
          type="text"
          placeholder="Buscar por nombre o descripción..."
          value={buscar}
          onChange={(e) => setBuscar(e.target.value)}
          className="flex-1 rounded border border-borde bg-hoja px-3 py-2 text-sm focus:border-acento focus:outline-none focus:ring-1 focus:ring-acento"
        />
        <select
          value={filtroActiva === undefined ? '' : String(filtroActiva)}
          onChange={(e) => {
            const value = e.target.value
            setFiltroActiva(value === '' ? undefined : value === 'true')
          }}
          className="rounded border border-borde bg-hoja px-3 py-2 text-sm focus:border-acento focus:outline-none focus:ring-1 focus:ring-acento"
        >
          <option value="">Todos los estados</option>
          <option value="true">Activas</option>
          <option value="false">Inactivas</option>
        </select>
      </div>

      {cargando && <Loading texto="Cargando materias..." />}

      {!cargando && error && <ErrorMessage mensaje={error} onReintentar={cargar} />}

      {!cargando && !error && materias.length === 0 && (
        <EmptyState mensaje="No hay materias todavía. Creá la primera con el botón de arriba." />
      )}

      {!cargando && !error && materias.length > 0 && (
        <div className="overflow-hidden rounded border border-borde bg-hoja">
          <div className="overflow-x-auto">
            <table className="min-w-full text-left text-sm">
              <thead className="border-b border-borde bg-papel text-tinta-suave">
                <tr>
                  <th className="px-4 py-3 font-medium">Materia</th>
                  <th className="px-4 py-3 font-medium">Descripción</th>
                  <th className="px-4 py-3 font-medium">Alumnos</th>
                  <th className="px-4 py-3 font-medium">Estado</th>
                  <th className="px-4 py-3 font-medium text-right">Acciones</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-borde">
                {materias.map((materia) => (
                  <tr key={materia.id} className="hover:bg-acento-claro/50">
                    <td className="px-4 py-3">
                      <Link
                        to={`/materias/${materia.id}`}
                        className="font-medium text-acento hover:underline"
                      >
                        {materia.nombre}
                      </Link>
                    </td>
                    <td className="px-4 py-3 text-tinta-suave">
                      {materia.descripcion ?? '—'}
                    </td>
                    <td className="px-4 py-3 text-tinta-suave">
                      {materia.cantidadAlumnos}
                    </td>
                    <td className="px-4 py-3">
                      <span
                        className={`inline-flex rounded px-2 py-0.5 text-xs font-medium ${
                          materia.activa
                            ? 'bg-exito/10 text-exito'
                            : 'bg-tinta/5 text-tinta-suave'
                        }`}
                      >
                        {materia.activa ? 'Activa' : 'Inactiva'}
                      </span>
                    </td>
                    <td className="px-4 py-3 text-right">
                      <button
                        type="button"
                        onClick={() => toggleEstado(materia)}
                        className={`rounded border px-3 py-1 text-xs font-medium ${
                          materia.activa
                            ? 'border-borde bg-hoja text-tinta hover:bg-acento-claro'
                            : 'border-acento bg-acento text-hoja hover:bg-tinta'
                        }`}
                      >
                        {materia.activa ? 'Desactivar' : 'Reactivar'}
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}
    </div>
  )
}
