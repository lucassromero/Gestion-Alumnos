export function EstadoBadge({ activo }: { activo: boolean }) {
  return (
    <span
      className={`inline-flex rounded px-2 py-0.5 text-xs font-medium ${
        activo
          ? 'bg-exito/10 text-exito'
          : 'bg-tinta/5 text-tinta-suave'
      }`}
    >
      {activo ? 'Activo' : 'Inactivo'}
    </span>
  )
}
