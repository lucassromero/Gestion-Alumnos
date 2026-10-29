import { useEffect, useState } from 'react'
import { useParams } from 'react-router-dom'
import { AlumnoForm } from '../components/AlumnoForm'
import { ErrorMessage } from '../components/ErrorMessage'
import { Loading } from '../components/Loading'
import { MateriasDelAlumno } from '../components/MateriasDelAlumno'
import {
  actualizarAlumno,
  cambiarEstadoAlumno,
  obtenerAlumno,
} from '../services/api'
import type { Alumno, AlumnoUpdate } from '../types/alumno'

export function DetalleAlumnoPage() {
  const { id } = useParams<{ id: string }>()

  const [alumno, setAlumno] = useState<Alumno | null>(null)
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
      const data = await obtenerAlumno(id)
      setAlumno(data)
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Error al cargar el alumno')
    } finally {
      setCargando(false)
    }
  }

  useEffect(() => {
    cargar()
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [id])

  async function handleSubmit(dto: AlumnoUpdate) {
    if (!id) return
    setEstadoGuardar('submitting')
    setErrorGuardar(undefined)
    try {
      const actualizado = await actualizarAlumno(id, dto)
      setAlumno(actualizado)
      setModoEdicion(false)
    } catch (err) {
      setEstadoGuardar('error')
      setErrorGuardar(err instanceof Error ? err.message : 'Error al guardar')
    }
  }

  async function handleToggleEstado() {
    if (!id || !alumno) return
    try {
      const actualizado = await cambiarEstadoAlumno(id, !alumno.activo)
      setAlumno(actualizado)
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Error al cambiar el estado')
    }
  }

  if (cargando) return <Loading texto="Cargando alumno..." />
  if (error) return <ErrorMessage mensaje={error} onReintentar={cargar} />
  if (!alumno) return <ErrorMessage mensaje="No se encontró el alumno." />

  return (
    <div className="max-w-2xl mx-auto space-y-5">
      <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
        <h1 className="font-serif text-3xl">
          {alumno.apellido}, {alumno.nombre}
        </h1>
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
              alumno.activo
                ? 'border-borde bg-hoja text-tinta hover:bg-acento-claro'
                : 'border-acento bg-acento text-hoja hover:bg-tinta'
            }`}
          >
            {alumno.activo ? 'Desactivar' : 'Reactivar'}
          </button>
        </div>
      </div>

      {modoEdicion ? (
        <AlumnoForm
          valoresIniciales={{
            nombre: alumno.nombre,
            apellido: alumno.apellido,
            email: alumno.email ?? undefined,
            telefono: alumno.telefono ?? undefined,
            nivelEducativo: alumno.nivelEducativo ?? undefined,
            observaciones: alumno.observaciones ?? undefined,
          }}
          onSubmit={handleSubmit}
          onCancelar={() => setModoEdicion(false)}
          estado={estadoGuardar}
          error={errorGuardar}
        />
      ) : (
        <div className="space-y-4 rounded border border-borde bg-hoja p-6">
          <div className="grid gap-4 sm:grid-cols-2">
            <div>
              <p className="text-xs text-tinta-suave">Email</p>
              <p className="text-sm text-tinta">{alumno.email ?? '—'}</p>
            </div>
            <div>
              <p className="text-xs text-tinta-suave">Teléfono</p>
              <p className="text-sm text-tinta">{alumno.telefono ?? '—'}</p>
            </div>
            <div>
              <p className="text-xs text-tinta-suave">Nivel educativo</p>
              <p className="text-sm text-tinta">{alumno.nivelEducativo ?? '—'}</p>
            </div>
            <div>
              <p className="text-xs text-tinta-suave">Estado</p>
              <p className="text-sm text-tinta">
                {alumno.activo ? 'Activo' : 'Inactivo'}
              </p>
            </div>
          </div>
          {alumno.observaciones && (
            <div>
              <p className="text-xs text-tinta-suave">Observaciones</p>
              <p className="mt-1 whitespace-pre-wrap text-sm text-tinta">
                {alumno.observaciones}
              </p>
            </div>
          )}
        </div>
      )}

      <MateriasDelAlumno idAlumno={alumno.id} />
    </div>
  )
}
