import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { MateriaForm } from '../components/MateriaForm'
import { crearMateria } from '../services/api'
import type { MateriaCreate } from '../types/materia'

export function NuevaMateriaPage() {
  const navigate = useNavigate()
  const [estado, setEstado] = useState<'idle' | 'submitting' | 'error'>('idle')
  const [error, setError] = useState<string | undefined>()

  async function handleSubmit(dto: MateriaCreate) {
    setEstado('submitting')
    setError(undefined)
    try {
      await crearMateria(dto)
      navigate('/materias')
    } catch (err) {
      setEstado('error')
      setError(err instanceof Error ? err.message : 'Error al guardar la materia')
    }
  }

  return (
    <div className="max-w-2xl mx-auto space-y-5">
      <h1 className="font-serif text-3xl">Nueva materia</h1>
      <MateriaForm
        onSubmit={handleSubmit}
        onCancelar={() => navigate('/materias')}
        estado={estado}
        error={error}
      />
    </div>
  )
}
