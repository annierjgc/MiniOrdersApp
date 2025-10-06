<template>
  <div class="p-4">
    <h2 class="text-2xl font-bold mb-4">Gestión de Órdenes</h2>

    <div v-if="errorMessage" class="alert alert-danger mb-3">{{ errorMessage }}</div>

    <!-- Botón para abrir modal -->
    <button class="btn btn-primary mb-3" @click="openModal()"> Nueva Orden</button>

    <!-- Tabla -->
    <table class="table table-bordered table-hover">
      <thead>
        <tr>
          <th>ID</th>
          <th>Cliente</th>
          <th>Fecha</th>
          <th>Cantidad</th>
          <th>Total</th>
          <th>Acciones</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="order in orders" :key="order.id">
          <td>{{ order.id }}</td>
          <td>{{ order.cliente }}</td>
          <td>{{ new Date(order.fecha).toLocaleDateString() }}</td>
          <td>{{ order.cantidad }}</td>
          <td>{{ order.total.toLocaleString('es-DO', { style: 'currency', currency: 'DOP' }) }}</td>
          <td>
            <button class="btn btn-sm btn-warning me-2" @click="openModal(order)">Editar</button>
            <button class="btn btn-sm btn-danger" @click="deleteOrder(order.id)">Eliminar</button>
          </td>
        </tr>
      </tbody>
    </table>

    <!-- MODAL -->
    <div class="modal fade" id="orderModal" tabindex="-1" ref="modalElement">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">{{ editing ? 'Editar Orden' : 'Nueva Orden' }}</h5>
            <button type="button" class="btn-close" data-bs-dismiss="modal" @click="resetForm"></button>

          </div>
          <form @submit.prevent="saveOrder" autocomplete="off">
            <div class="modal-body">
              <div class="mb-3">
                <label class="form-label">Cliente</label>
                <input v-model="form.cliente" class="form-control" required />
              </div>
              <div class="mb-3">
                <label class="form-label">Fecha</label>
                <input type="date" v-model="form.fecha" class="form-control" required />
              </div>
              <div class="mb-3">
                <label class="form-label">Cantidad</label>
                <input type="number" v-model="form.cantidad" class="form-control" required />
              </div>
              <div class="mb-3">
                <label class="form-label">Total</label>
                <input type="number" v-model="form.total" class="form-control" required />
              </div>
            </div>

                <div class="modal-footer">
                <button type="submit" class="btn btn-primary">{{ editing ? 'Actualizar' : 'Guardar' }}</button>
              </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, nextTick } from 'vue'
import axios from 'axios'
import * as bootstrap from 'bootstrap'

const API_URL = 'http://localhost:5176/api/orders'
const orders = ref([])
const editing = ref(false)
const today = new Date().toISOString().split('T')[0]
const form = ref({ Id: null, Cliente: '', Fecha: today, Cantidad: 1, Total: 0 })
const modalElement = ref(null)
const errorMessage = ref('')
const modalInstance = ref(null)

onMounted(async () => {
  await fetchOrders()
  await nextTick()
  modalInstance.value = new bootstrap.Modal(modalElement.value)
})

const fetchOrders = async () => {
  try {
    const res = await axios.get(API_URL)
    orders.value = res.data
    errorMessage.value = ''
  } catch (err) {
    console.error('Error al obtener órdenes:', err)
    errorMessage.value = 'No se pudieron obtener las órdenes. Intente más tarde.'
  }
}

const openModal = (order = null) => {
  if (order) {
    form.value = { ...order, fecha: order.fecha.split('T')[0] }
    editing.value = true
  } else {
    resetForm()
  }
  modalInstance.value.show()
}

const saveOrder = async () => {
  try {
    const orderPayload = {
      Id: form.value.id ?? "00000000-0000-0000-0000-000000000000",
      Cliente: form.value.cliente,
      Fecha: new Date(form.value.fecha).toISOString(), // formato correcto ISO
      Cantidad: Number(form.value.cantidad),
      Total: Number(form.value.total)
    };

    if (editing.value) {
      await axios.put(`${API_URL}/${form.value.id}`, orderPayload);
    } else {
      await axios.post(API_URL, orderPayload);
    }

    await fetchOrders();
    modalInstance.value.hide();
    resetForm();
    errorMessage.value = '';
  } catch (err) {
    console.error('Error al guardar la orden:', err.response?.data || err.message);
    errorMessage.value = 'No se pudo guardar la orden. Revise los datos e intente nuevamente.';
  }
};


const deleteOrder = async (id) => {
  if (!confirm('¿Estás seguro de eliminar esta orden?')) return
  try {
    await axios.delete(`${API_URL}/${id}`)
    await fetchOrders()
    errorMessage.value = ''
  } catch (err) {
    console.error('Error al eliminar la orden:', err)
    errorMessage.value = 'No se pudo eliminar la orden. Intente más tarde.'
  }
}

const resetForm = () => {
  form.value = { id: null, cliente: '', fecha: '', cantidad: 1, total: 0 }
  editing.value = false
}
</script>


<style scoped>
.table {
  border-collapse: collapse;
  width: 100%;
}
.table th,
.table td {
  border: 1px solid #6b6666;
  padding: 8px;
}
.table th {
  background-color: #b7b7b7;
}
</style>
