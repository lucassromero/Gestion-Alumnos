import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import {
  asociarMateria,
  desasociarMateria,
  listarMaterias,
  listarMateriasDeAlumno,
} from '../services/api'
import type { Materia } from '../types/materia'

interface MateriasDelAlumnoProps {
  idAlumno: string
}

export function MateriasDelAlumno({ idAlumno }: MateriasDelAlumnoProps) {
  const [asociadas, setAsociadas] = useState<Materia[]>([])
  const [disponibles, setDisponibles] = useState<Materia[]>([])
  const [seleccionada, setSeleccionada] = useState('')
  const [cargando, setCargando] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [procesando, setProcesando] = useState(false)

  async function cargar() {
    setCargando(true)
    setError(null)
    try {
      const [deAlumno, activas] = await Promise.all([
        listarMateriasDeAlumno(idAlumno),
        listarMaterias({ activa: true }),
      ])
      setAsociadas(deAlumno)
      setDisponibles(activas)
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Error al cargar las materias')
    } finally {
      setCargando(false)
    }
  }

  useEffect(() => {
    cargar()
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [idAlumno])

  const sinAsociar = disponibles.filter(
    (materia) => !asociadas.some((asociada) => asociada.id === materia.id)
  )

  async function handleAsociar() {
    if (!seleccionada) return
    setProcesando(true)
    setError(null)
    try {
      await asociarMateria(seleccionada, idAlumno)
      setSeleccionada('')
      await cargar()
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Error al asociar la materia')
    } finally {
      setProcesando(false)
    }
  }

  async function handleDesasociar(idMateria: string) {
    setProcesando(true)
    setError(null)
    try {
      await desasociarMateria(idMateria, idAlumno)
      await cargar()
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Error al desasociar la materia')
    } finally {
      setProcesando(false)
    }
  }

  return (
    <section className="space-y-4 rounded border border-borde bg-hoja p-6">
      <div>
        <h2 className="font-serif text-xl">Materias asociadas</h2>
        <p className="mt-1 text-sm text-tinta-suave">
          Materias que este alumno está cursando.
        </p>
      </div>

      {cargando && <p className="text-sm text-tinta-suave">Cargando materias...</p>}

      {!cargando && error && (
        <div className="rounded border border-error/20 bg-error/5 p-3 text-sm text-error">
          <p>{error}</p>
          <button
            type="button"
            onClick={cargar}
            className="mt-2 rounded border border-error/30 px-3 py-1 text-xs font-medium text-error hover:bg-error/10"
          >
            Reintentar
          </button>
        </div>
      )}

      {!cargando && !error && asociadas.length === 0 && (
        <p className="text-sm text-tinta-suave">
          Todavía no tiene materias asociadas.
        </p>
      )}

      {!cargando && asociadas.length > 0 && (
        <ul className="divide-y divide-borde rounded border border-borde">
          {asociadas.map((materia) => (
            <li
              key={materia.id}
              className="flex flex-col gap-2 px-4 py-3 sm:flex-row sm:items-center sm:justify-between"
            >
              <div>
                <Link
                  to={`/materias/${materia.id}`}
                  className="text-sm font-medium text-acento hover:underline"
                >
                  {materia.nombre}
                </Link>
                {!materia.activa && (
                  <span className="ml-2 rounded bg-tinta/5 px-2 py-0.5 text-xs font-medium text-tinta-suave">
                    Inactiva
                  </span>
                )}
              </div>
              <button
                type="button"
                onClick={() => handleDesasociar(materia.id)}
                disabled={procesando}
                className="self-start rounded border border-borde bg-hoja px-3 py-1 text-xs font-medium text-tinta hover:bg-acento-claro disabled:opacity-60 sm:self-auto"
              >
                Desasociar
              </button>
            </li>
          ))}
        </ul>
      )}

      {!cargando && (
        <div className="border-t border-borde pt-4">
          <label htmlFor="materia-disponible" className="block text-sm font-medium text-tinta">
            Asociar una materia
          </label>
          {sinAsociar.length === 0 ? (
            <p className="mt-2 text-sm text-tinta-suave">
              No hay materias activas disponibles para asociar.
            </p>
          ) : (
            <div className="mt-2 flex flex-col gap-2 sm:flex-row">
              <select
                id="materia-disponible"
                value={seleccionada}
                onChange={(e) => setSeleccionada(e.target.value)}
                disabled={procesando}
                className="flex-1 rounded border border-borde bg-papel px-3 py-2 text-sm focus:border-acento focus:outline-none focus:ring-1 focus:ring-acento disabled:opacity-60"
              >
                <option value="">Seleccioná una materia...</option>
                {sinAsociar.map((materia) => (
                  <option key={materia.id} value={materia.id}>
                    {materia.nombre}
                  </option>
                ))}
              </select>
              <button
                type="button"
                onClick={handleAsociar}
                disabled={!seleccionada || procesando}
                className="rounded bg-acento px-4 py-2 text-sm font-medium text-hoja hover:bg-tinta disabled:opacity-60"
              >
                {procesando ? 'Asociando...' : 'Asociar'}
              </button>
            </div>
          )}
        </div>
      )}
    </section>
  )
}
