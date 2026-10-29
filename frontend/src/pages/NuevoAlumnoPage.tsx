import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { AlumnoForm } from '../components/AlumnoForm'
import { crearAlumno } from '../services/api'
import type { AlumnoCreate } from '../types/alumno'

export function NuevoAlumnoPage() {
  const navigate = useNavigate()
  const [estado, setEstado] = useState<'idle' | 'submitting' | 'error'>('idle')
  const [error, setError] = useState<string | undefined>()

  async function handleSubmit(dto: AlumnoCreate) {
    setEstado('submitting')
    setError(undefined)
    try {
      await crearAlumno(dto)
      navigate('/alumnos')
    } catch (err) {
      setEstado('error')
      setError(err instanceof Error ? err.message : 'Error al guardar el alumno')
    }
  }

  return (
    <div className="max-w-2xl mx-auto space-y-5">
      <h1 className="font-serif text-3xl">Nuevo alumno</h1>
      <AlumnoForm
        onSubmit={handleSubmit}
        onCancelar={() => navigate('/alumnos')}
        estado={estado}
        error={error}
      />
    </div>
  )
}
