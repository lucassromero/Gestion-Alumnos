import { useState } from 'react'
import type { MateriaCreate } from '../types/materia'

interface MateriaFormProps {
  valoresIniciales?: Partial<MateriaCreate>
  onSubmit: (dto: MateriaCreate) => void
  onCancelar: () => void
  estado: 'idle' | 'submitting' | 'error'
  error?: string
}

export function MateriaForm({
  valoresIniciales,
  onSubmit,
  onCancelar,
  estado,
  error,
}: MateriaFormProps) {
  const [dto, setDto] = useState<MateriaCreate>({
    nombre: valoresIniciales?.nombre ?? '',
    descripcion: valoresIniciales?.descripcion ?? '',
  })

  const [errores, setErrores] = useState<Record<string, string>>({})

  function validar(): boolean {
    const nuevos: Record<string, string> = {}
    if (!dto.nombre.trim()) nuevos.nombre = 'El nombre es obligatorio.'
    setErrores(nuevos)
    return Object.keys(nuevos).length === 0
  }

  function handleChange(
    e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>
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
    <form onSubmit={handleSubmit} className="space-y-5 rounded border border-borde bg-hoja p-6">
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
        <label htmlFor="descripcion" className="block text-sm font-medium text-tinta">
          Descripción
        </label>
        <textarea
          id="descripcion"
          name="descripcion"
          rows={3}
          value={dto.descripcion}
          onChange={handleChange}
          className="mt-1.5 block w-full rounded border border-borde bg-papel px-3 py-2 text-sm focus:border-acento focus:outline-none focus:ring-1 focus:ring-acento"
        />
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
