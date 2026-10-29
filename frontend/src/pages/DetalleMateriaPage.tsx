import { useEffect, useState } from 'react'
import { Link, useParams } from 'react-router-dom'
import { ErrorMessage } from '../components/ErrorMessage'
import { Loading } from '../components/Loading'
import { MateriaForm } from '../components/MateriaForm'
import {
  actualizarMateria,
  cambiarEstadoMateria,
  obtenerMateria,
} from '../services/api'
import type { MateriaDetalle, MateriaUpdate } from '../types/materia'

export function DetalleMateriaPage() {
  const { id } = useParams<{ id: string }>()

  const [materia, setMateria] = useState<MateriaDetalle | null>(null)
  const [cargando, setCargando] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [modoEdicion, setModoEdicion] = useState(false)
  const [estadoGuardar, setEstadoGuardar] = useState<'idle' | 'submitting' | 'error'>('idle')
  const [errorGuardar, setErrorGuardar] = useState<string | undefined>()

  async function cargar() {
    if (!id) return
    setCargando(true)
    setError(null)
    try {
      const data = await obtenerMateria(id)
      setMateria(data)
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Error al cargar la materia')
    } finally {
      setCargando(false)
    }
  }

  useEffect(() => {
    cargar()
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [id])

  async function handleSubmit(dto: MateriaUpdate) {
    if (!id) return
    setEstadoGuardar('submitting')
    setErrorGuardar(undefined)
    try {
      await actualizarMateria(id, dto)
      setModoEdicion(false)
      await cargar()
    } catch (err) {
      setEstadoGuardar('error')
      setErrorGuardar(err instanceof Error ? err.message : 'Error al guardar')
    }
  }

  async function handleToggleEstado() {
    if (!id || !materia) return
    try {
      await cambiarEstadoMateria(id, !materia.activa)
      await cargar()
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Error al cambiar el estado')
    }
  }

  if (cargando) return <Loading texto="Cargando materia..." />
  if (error) return <ErrorMessage mensaje={error} onReintentar={cargar} />
  if (!materia) return <ErrorMessage mensaje="No se encontró la materia." />

  return (
    <div className="max-w-2xl mx-auto space-y-5">
      <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h1 className="font-serif text-3xl">{materia.nombre}</h1>
          <p className="mt-1 text-sm text-tinta-suave">
            {materia.activa ? 'Activa' : 'Inactiva'}
          </p>
        </div>
        <div className="flex gap-2">
          <button
            type="button"
            onClick={() => setModoEdicion((v) => !v)}
            className="rounded border border-borde bg-hoja px-3 py-1.5 text-sm font-medium text-tinta hover:bg-acento-claro"
          >
            {modoEdicion ? 'Cancelar edición' : 'Editar'}
          </button>
          <button
            type="button"
            onClick={handleToggleEstado}
            className={`rounded border px-3 py-1.5 text-sm font-medium ${
              materia.activa
                ? 'border-borde bg-hoja text-tinta hover:bg-acento-claro'
                : 'border-acento bg-acento text-hoja hover:bg-tinta'
            }`}
          >
            {materia.activa ? 'Desactivar' : 'Reactivar'}
          </button>
        </div>
      </div>

      {modoEdicion ? (
        <MateriaForm
          valoresIniciales={{
            nombre: materia.nombre,
            descripcion: materia.descripcion ?? undefined,
          }}
          onSubmit={handleSubmit}
          onCancelar={() => setModoEdicion(false)}
          estado={estadoGuardar}
          error={errorGuardar}
        />
      ) : (
        <div className="space-y-5 rounded border border-borde bg-hoja p-6">
          {materia.descripcion && (
            <div>
              <p className="text-xs text-tinta-suave">Descripción</p>
              <p className="mt-1 whitespace-pre-wrap text-sm text-tinta">
                {materia.descripcion}
              </p>
            </div>
          )}

          <div>
            <h2 className="font-serif text-xl">Alumnos asociados</h2>
            {materia.alumnos.length === 0 ? (
              <p className="mt-2 text-sm text-tinta-suave">
                Todavía no hay alumnos asociados a esta materia.
              </p>
            ) : (
              <ul className="mt-3 divide-y divide-borde rounded border border-borde">
                {materia.alumnos.map((alumno) => (
                  <li key={alumno.id} className="flex items-center justify-between px-4 py-3">
                    <Link
                      to={`/alumnos/${alumno.id}`}
                      className="text-sm font-medium text-acento hover:underline"
                    >
                      {alumno.apellido}, {alumno.nombre}
                    </Link>
                    <span className="text-xs text-tinta-suave">
                      {alumno.activo ? 'Activo' : 'Inactivo'}
                    </span>
                  </li>
                ))}
              </ul>
            )}
          </div>
        </div>
      )}
    </div>
  )
}
