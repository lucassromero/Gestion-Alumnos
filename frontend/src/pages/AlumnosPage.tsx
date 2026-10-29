import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { EmptyState } from '../components/EmptyState'
import { ErrorMessage } from '../components/ErrorMessage'
import { EstadoBadge } from '../components/EstadoBadge'
import { Loading } from '../components/Loading'
import { cambiarEstadoAlumno, listarAlumnos } from '../services/api'
import type { Alumno } from '../types/alumno'

export function AlumnosPage() {
  const [alumnos, setAlumnos] = useState<Alumno[]>([])
  const [cargando, setCargando] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [filtroActivo, setFiltroActivo] = useState<boolean | undefined>(undefined)
  const [buscar, setBuscar] = useState('')

  async function cargar() {
    setCargando(true)
    setError(null)
    try {
      const data = await listarAlumnos({
        activo: filtroActivo,
        buscar: buscar.trim() || undefined,
      })
      setAlumnos(data)
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Error desconocido')
    } finally {
      setCargando(false)
    }
  }

  useEffect(() => {
    cargar()
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [filtroActivo])

  useEffect(() => {
    const timeout = setTimeout(() => {
      cargar()
    }, 300)
    return () => clearTimeout(timeout)
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [buscar])

  async function toggleEstado(alumno: Alumno) {
    try {
      const actualizado = await cambiarEstadoAlumno(alumno.id, !alumno.activo)
      setAlumnos((prev) => prev.map((a) => (a.id === actualizado.id ? actualizado : a)))
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Error al cambiar el estado')
    }
  }

  return (
    <div className="space-y-6">
      <div className="flex flex-col gap-4 sm:flex-row sm:items-end sm:justify-between">
        <div>
          <h1 className="font-serif text-3xl">Alumnos</h1>
          <p className="mt-1 text-sm text-tinta-suave">
            {alumnos.length} {alumnos.length === 1 ? 'registro' : 'registros'}
          </p>
        </div>
        <Link
          to="/alumnos/nuevo"
          className="inline-flex justify-center rounded bg-acento px-4 py-2 text-sm font-medium text-hoja hover:bg-tinta"
        >
          Nuevo alumno
        </Link>
      </div>

      <div className="flex flex-col gap-3 sm:flex-row">
        <input
          type="text"
          placeholder="Buscar por nombre, apellido o email..."
          value={buscar}
          onChange={(e) => setBuscar(e.target.value)}
          className="flex-1 rounded border border-borde bg-hoja px-3 py-2 text-sm focus:border-acento focus:outline-none focus:ring-1 focus:ring-acento"
        />
        <select
          value={filtroActivo === undefined ? '' : String(filtroActivo)}
          onChange={(e) => {
            const value = e.target.value
            setFiltroActivo(value === '' ? undefined : value === 'true')
          }}
          className="rounded border border-borde bg-hoja px-3 py-2 text-sm focus:border-acento focus:outline-none focus:ring-1 focus:ring-acento"
        >
          <option value="">Todos los estados</option>
          <option value="true">Activos</option>
          <option value="false">Inactivos</option>
        </select>
      </div>

      {cargando && <Loading texto="Cargando alumnos..." />}

      {!cargando && error && <ErrorMessage mensaje={error} onReintentar={cargar} />}

      {!cargando && !error && alumnos.length === 0 && (
        <EmptyState mensaje="No se encontraron alumnos. Creá el primero con el botón de arriba." />
      )}

      {!cargando && !error && alumnos.length > 0 && (
        <div className="overflow-hidden rounded border border-borde bg-hoja">
          <div className="overflow-x-auto">
            <table className="min-w-full text-left text-sm">
              <thead className="border-b border-borde bg-papel text-tinta-suave">
                <tr>
                  <th className="px-4 py-3 font-medium">Apellido y nombre</th>
                  <th className="px-4 py-3 font-medium">Contacto</th>
                  <th className="px-4 py-3 font-medium">Nivel educativo</th>
                  <th className="px-4 py-3 font-medium">Estado</th>
                  <th className="px-4 py-3 font-medium text-right">Acciones</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-borde">
                {alumnos.map((alumno) => (
                  <tr key={alumno.id} className="hover:bg-acento-claro/50">
                    <td className="px-4 py-3">
                      <Link
                        to={`/alumnos/${alumno.id}`}
                        className="font-medium text-acento hover:underline"
                      >
                        {alumno.apellido}, {alumno.nombre}
                      </Link>
                    </td>
                    <td className="px-4 py-3 text-tinta-suave">
                      {alumno.email ?? '—'}
                      {alumno.telefono && (
                        <div className="text-xs">{alumno.telefono}</div>
                      )}
                    </td>
                    <td className="px-4 py-3 text-tinta-suave">
                      {alumno.nivelEducativo ?? '—'}
                    </td>
                    <td className="px-4 py-3">
                      <EstadoBadge activo={alumno.activo} />
                    </td>
                    <td className="px-4 py-3 text-right">
                      <button
                        type="button"
                        onClick={() => toggleEstado(alumno)}
                        className={`rounded border px-3 py-1 text-xs font-medium ${
                          alumno.activo
                            ? 'border-borde bg-hoja text-tinta hover:bg-acento-claro'
                            : 'border-acento bg-acento text-hoja hover:bg-tinta'
                        }`}
                      >
                        {alumno.activo ? 'Desactivar' : 'Reactivar'}
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
