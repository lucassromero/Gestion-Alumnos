import { useState } from 'react'
import type { AlumnoCreate } from '../types/alumno'

interface AlumnoFormProps {
  valoresIniciales?: Partial<AlumnoCreate>
  onSubmit: (dto: AlumnoCreate) => void
  onCancelar: () => void
  estado: 'idle' | 'submitting' | 'error'
  error?: string
}

export function AlumnoForm({
  valoresIniciales,
  onSubmit,
  onCancelar,
  estado,
  error,
}: AlumnoFormProps) {
  const [dto, setDto] = useState<AlumnoCreate>({
    nombre: valoresIniciales?.nombre ?? '',
    apellido: valoresIniciales?.apellido ?? '',
    email: valoresIniciales?.email ?? '',
    telefono: valoresIniciales?.telefono ?? '',
    nivelEducativo: valoresIniciales?.nivelEducativo ?? '',
    observaciones: valoresIniciales?.observaciones ?? '',
  })

  const [errores, setErrores] = useState<Record<string, string>>({})

  function validar(): boolean {
    const nuevos: Record<string, string> = {}
    if (!dto.nombre.trim()) nuevos.nombre = 'El nombre es obligatorio.'
    if (!dto.apellido.trim()) nuevos.apellido = 'El apellido es obligatorio.'
    if (dto.email && !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(dto.email)) {
      nuevos.email = 'El formato del email no es válido.'
    }
    setErrores(nuevos)
    return Object.keys(nuevos).length === 0
  }

  function handleChange(
    e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>
  ) {
    const { name, value } = e.target
    setDto((prev) => ({ ...prev, [name]: value }))
    setErrores((prev) => {
      const copy = { ...prev }
      delete copy[name]
      return copy
    })
  }

  function handleSubmit(e: React.FormEvent) {
    e.preventDefault()
    if (!validar()) return
    onSubmit(dto)
  }

  return (
    <form onSubmit={handleSubmit} className="space-y-5 bg-hoja border border-borde p-6 rounded">
      <div className="grid gap-5 sm:grid-cols-2">
        <div>
          <label htmlFor="nombre" className="block text-sm font-medium text-tinta">
            Nombre <span className="text-error" aria-hidden="true">*</span>
          </label>
          <input
            id="nombre"
            name="nombre"
            type="text"
            value={dto.nombre}
            onChange={handleChange}
            className="mt-1.5 block w-full rounded border border-borde bg-papel px-3 py-2 text-sm focus:border-acento focus:outline-none focus:ring-1 focus:ring-acento"
            aria-invalid={!!errores.nombre}
            aria-describedby={errores.nombre ? 'nombre-error' : undefined}
          />
          {errores.nombre && (
            <p id="nombre-error" className="mt-1 text-xs text-error">
              {errores.nombre}
            </p>
          )}
        </div>

        <div>
          <label htmlFor="apellido" className="block text-sm font-medium text-tinta">
            Apellido <span className="text-error" aria-hidden="true">*</span>
          </label>
          <input
            id="apellido"
            name="apellido"
            type="text"
            value={dto.apellido}
            onChange={handleChange}
            className="mt-1.5 block w-full rounded border border-borde bg-papel px-3 py-2 text-sm focus:border-acento focus:outline-none focus:ring-1 focus:ring-acento"
            aria-invalid={!!errores.apellido}
            aria-describedby={errores.apellido ? 'apellido-error' : undefined}
          />
          {errores.apellido && (
            <p id="apellido-error" className="mt-1 text-xs text-error">
              {errores.apellido}
            </p>
          )}
        </div>

        <div>
          <label htmlFor="email" className="block text-sm font-medium text-tinta">
            Email
          </label>
          <input
            id="email"
            name="email"
            type="email"
            value={dto.email}
            onChange={handleChange}
            className="mt-1.5 block w-full rounded border border-borde bg-papel px-3 py-2 text-sm focus:border-acento focus:outline-none focus:ring-1 focus:ring-acento"
            aria-invalid={!!errores.email}
            aria-describedby={errores.email ? 'email-error' : undefined}
          />
          {errores.email && (
            <p id="email-error" className="mt-1 text-xs text-error">
              {errores.email}
            </p>
          )}
        </div>

        <div>
          <label htmlFor="telefono" className="block text-sm font-medium text-tinta">
            Teléfono
          </label>
          <input
            id="telefono"
            name="telefono"
            type="text"
            value={dto.telefono}
            onChange={handleChange}
            className="mt-1.5 block w-full rounded border border-borde bg-papel px-3 py-2 text-sm focus:border-acento focus:outline-none focus:ring-1 focus:ring-acento"
          />
        </div>

        <div className="sm:col-span-2">
          <label htmlFor="nivelEducativo" className="block text-sm font-medium text-tinta">
            Nivel educativo
          </label>
          <input
            id="nivelEducativo"
            name="nivelEducativo"
            type="text"
            value={dto.nivelEducativo}
            onChange={handleChange}
            className="mt-1.5 block w-full rounded border border-borde bg-papel px-3 py-2 text-sm focus:border-acento focus:outline-none focus:ring-1 focus:ring-acento"
          />
        </div>

        <div className="sm:col-span-2">
          <label htmlFor="observaciones" className="block text-sm font-medium text-tinta">
            Observaciones
          </label>
          <textarea
            id="observaciones"
            name="observaciones"
            rows={3}
            value={dto.observaciones}
            onChange={handleChange}
            className="mt-1.5 block w-full rounded border border-borde bg-papel px-3 py-2 text-sm focus:border-acento focus:outline-none focus:ring-1 focus:ring-acento"
          />
        </div>
      </div>

      {error && (
        <div className="rounded border border-error/20 bg-error/5 p-3 text-sm text-error">
          {error}
        </div>
      )}

      <div className="flex items-center justify-end gap-3 pt-2">
        <button
          type="button"
          onClick={onCancelar}
          className="rounded border border-borde bg-hoja px-4 py-2 text-sm font-medium text-tinta hover:bg-acento-claro"
        >
          Cancelar
        </button>
        <button
          type="submit"
          disabled={estado === 'submitting'}
          className="rounded bg-acento px-4 py-2 text-sm font-medium text-hoja hover:bg-tinta disabled:opacity-60"
        >
          {estado === 'submitting' ? 'Guardando...' : 'Guardar'}
        </button>
      </div>
    </form>
  )
}
